using AutoMapper;
using SportTrack_v1.Controladores.Fase.Dtos;
using SportTrack_v1.Controladores.Inscripcion;
using SportTrack_v1.Controladores.Evento;
using SportTrack_v1.Entidades.Entidades;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Fase
{
    public interface IFaseService
    {
        Task<IEnumerable<FaseDto>> GetFasesPorEventoPruebaAsync(int eventoPruebaId);
        Task<IEnumerable<FaseDto>> GenerarFasesAutoAsync(int eventoPruebaId);
        Task<IEnumerable<FaseDto>> PromoverFasesAsync(int eventoPruebaId);
        Task<FaseDto> IniciarFaseAsync(int id, DateTime? manualStartTime = null);
        Task<FaseDto> FinalizarFaseAsync(int id);
        Task<bool> DeleteFaseAsync(int id);
        Task<FaseDto> ReiniciarFaseAsync(int id);
        Task<FaseDto> EnviarARevisionAsync(int id);
        Task<IEnumerable<FaseDto>> GetFasesPorEventoAsync(int eventoId);
        Task BatchUpdateFasesAsync(List<FaseBatchUpdateDto> dto);
        Task<IEnumerable<FaseDto>> GenerarFasesManualAsync(int eventoPruebaId, List<ManualPlacementDto> placements);
        Task UpdateResultadoStatusAsync(int resultadoId, string status);
    }

    public class FaseService : IFaseService
    {
        private readonly IFaseRepository _faseRepository;
        private readonly IEtapaRepository _etapaRepository;
        private readonly IInscripcionRepository _inscripcionRepository;
        private readonly IEventoRepository _eventoRepository;

        private readonly Microsoft.AspNetCore.SignalR.IHubContext<SportTrack_v1.Controladores.Hubs.TimingHub> _hubContext;
        private readonly IMapper _mapper;
        private readonly Audit.IAuditService _auditService;

        public FaseService(
            IFaseRepository faseRepository, 
            IEtapaRepository etapaRepository,
            IInscripcionRepository inscripcionRepository, 
            IEventoRepository eventoRepository,
            Microsoft.AspNetCore.SignalR.IHubContext<SportTrack_v1.Controladores.Hubs.TimingHub> hubContext,
            IMapper mapper,
            Audit.IAuditService auditService)
        {
            _faseRepository = faseRepository;
            _etapaRepository = etapaRepository;
            _inscripcionRepository = inscripcionRepository;
            _eventoRepository = eventoRepository;
            _hubContext = hubContext;

            _mapper = mapper;
            _auditService = auditService;
        }

        public async Task<IEnumerable<FaseDto>> GetFasesPorEventoPruebaAsync(int eventoPruebaId)
        {
            var fases = await _faseRepository.GetByEventoPruebaIdAsync(eventoPruebaId);
            return _mapper.Map<IEnumerable<FaseDto>>(fases);
        }

        public async Task<IEnumerable<FaseDto>> GenerarFasesAutoAsync(int eventoPruebaId)
        {
            // Verificación de seguridad: No permitir re-sortear si ya hay resultados oficiales
            var fasesExistentes = await _faseRepository.GetByEventoPruebaIdAsync(eventoPruebaId);
            if (fasesExistentes.Any(f => f.Resultados.Any(r => r.TiempoOficial.HasValue)))
            {
                throw new InvalidOperationException("No se puede volver a sortear una regata que ya tiene resultados oficiales cargados.");
            }

            // 1. LIMPIEZA TOTAL
            await _etapaRepository.DeleteByEventoPruebaIdAsync(eventoPruebaId);
            
            var inscriptos = (await _inscripcionRepository.GetByEventoPruebaIdAsync(eventoPruebaId)).ToList();
            int inscriptosCount = inscriptos.Count;

            if (inscriptosCount == 0) return new List<FaseDto>();

            int numSeries = (int)Math.Ceiling(inscriptosCount / 9.0);
            
            var ep = await _eventoRepository.GetEventoPruebaByIdAsync(eventoPruebaId);
            
            // Asignar Plan de Progresión Automáticamente
            if (ep != null)
            {
                ep.PlanProgresionAsignado = DeterminarPlanProgresion(inscriptosCount);
                await _eventoRepository.UpdateEventoPruebaAsync(ep);
            }

            // ANCLAJE AL PROGRAMA: Usamos la hora programada del evento o de la prueba.
            DateTime nextTime;
            if (ep?.FechaHora != null)
            {
                nextTime = ep.FechaHora;
            }
            else
            {
                var baseDate = ep?.Evento?.Fecha.Date ?? DateTime.UtcNow.Date;
                var horaBase = ep?.Evento?.HoraInicioEvento ?? new TimeSpan(8, 0, 0);
                nextTime = GetUtcTime(baseDate.Add(horaBase), ep?.Evento?.TimeZoneId ?? "America/Argentina/Buenos_Aires");
            }

            if (numSeries <= 1)
            {
                var etapaFinal = new Etapa { 
                    EventoPruebaId = eventoPruebaId, 
                    Nombre = "Finales", 
                    Tipo = SportTrack_v1.Entidades.Enums.TipoEtapaEnum.Final, 
                    Orden = 3
                };
                await _etapaRepository.CreateAsync(etapaFinal);

                var faseFinal = new Entidades.Entidades.Fase
                {
                    EtapaId = etapaFinal.Id,
                    NombreFase = "Final A",
                    NumeroFase = 1,
                    Estado = "Programada",
                    FechaHoraProgramada = nextTime
                };

                var priorityLanes = new int[] { 5, 4, 6, 3, 7, 2, 8, 1, 9 };
                int laneIdx = 0;
                foreach (var ins in inscriptos.OrderByDescending(x => x.EsCabezaDeSerie).ThenBy(x => x.Id))
                {
                    faseFinal.Resultados.Add(new Entidades.Entidades.Resultado { InscripcionId = ins.Id, Carril = priorityLanes[laneIdx++] });
                }

                await _faseRepository.CreateAsync(faseFinal);
            }
            else
            {
                var etapaElim = new Etapa { 
                    EventoPruebaId = eventoPruebaId, 
                    Nombre = "Eliminatorias", 
                    Tipo = SportTrack_v1.Entidades.Enums.TipoEtapaEnum.Eliminatoria, 
                    Orden = 1 
                };
                await _etapaRepository.CreateAsync(etapaElim);

                var series = new List<List<Entidades.Entidades.Inscripcion>>();
                for (int i = 0; i < numSeries; i++) series.Add(new List<Entidades.Entidades.Inscripcion>());

                var queue = inscriptos.OrderByDescending(x => x.EsCabezaDeSerie).ThenBy(x => Guid.NewGuid()).ToList();
                int idx = 0;
                foreach (var ins in queue)
                {
                    series[idx % numSeries].Add(ins);
                    idx++;
                }

                for (int i = 0; i < numSeries; i++)
                {
                    var fase = new Entidades.Entidades.Fase
                    {
                        EtapaId = etapaElim.Id,
                        NombreFase = $"Serie {i + 1}",
                        NumeroFase = i + 1,
                        Estado = "Programada",
                        FechaHoraProgramada = nextTime
                    };

                    var priorityLanes = new int[] { 5, 4, 6, 3, 7, 2, 8, 1, 9 };
                    int laneIdx = 0;
                    foreach (var ins in series[i])
                    {
                        fase.Resultados.Add(new Entidades.Entidades.Resultado { InscripcionId = ins.Id, Carril = priorityLanes[laneIdx++] });
                    }

                    await _faseRepository.CreateAsync(fase);
                    nextTime = nextTime.AddMinutes(10);
                }

                await PreGenerarSiguientesEtapasAsync(eventoPruebaId, inscriptosCount, numSeries, nextTime);
            }
            
            // Auditoria
            await _auditService.RegistrarAccionAsync("GENERATE_HEATS_AUTO", 
                $"Sorteo automático generado para la Prueba ID {eventoPruebaId}. Se crearon {numSeries} series.", null, "Competencia");

            return await GetFasesPorEventoPruebaAsync(eventoPruebaId);
        }

        private async Task PreGenerarSiguientesEtapasAsync(int eventoPruebaId, int inscriptosCount, int numSeries, DateTime nextTime)
        {
            int numSemis = 0;
            if (numSeries == 2) numSemis = 1;
            else if (numSeries == 3) numSemis = 2;
            else if (numSeries >= 4) numSemis = 3;

            if (numSemis > 0)
            {
                var etapaSemi = new Etapa { 
                    EventoPruebaId = eventoPruebaId, 
                    Nombre = "Semifinales", 
                    Tipo = SportTrack_v1.Entidades.Enums.TipoEtapaEnum.Semifinal, 
                    Orden = 2 
                };
                await _etapaRepository.CreateAsync(etapaSemi);

                for (int i = 0; i < numSemis; i++)
                {
                    var faseSemi = new Entidades.Entidades.Fase
                    {
                        EtapaId = etapaSemi.Id,
                        NombreFase = numSemis > 1 ? $"Semifinal {i + 1}" : "Semifinal",
                        NumeroFase = i + 1,
                        Estado = "Programada",
                        FechaHoraProgramada = nextTime.AddMinutes(40)
                    };
                    await _faseRepository.CreateAsync(faseSemi);
                    nextTime = nextTime.AddMinutes(10);
                }
            }

            var etapaFinal = new Etapa { 
                EventoPruebaId = eventoPruebaId, 
                Nombre = "Finales", 
                Tipo = SportTrack_v1.Entidades.Enums.TipoEtapaEnum.Final, 
                Orden = 3 
            };
            await _etapaRepository.CreateAsync(etapaFinal);

            var faseFinalA = new Entidades.Entidades.Fase
            {
                EtapaId = etapaFinal.Id,
                NombreFase = "Final A",
                NumeroFase = 1,
                Estado = "Programada",
                FechaHoraProgramada = nextTime.AddMinutes(80)
            };
            await _faseRepository.CreateAsync(faseFinalA);

            if (numSeries >= 3)
            {
                var faseFinalB = new Entidades.Entidades.Fase
                {
                    EtapaId = etapaFinal.Id,
                    NombreFase = "Final B",
                    NumeroFase = 2,
                    Estado = "Programada",
                    FechaHoraProgramada = nextTime.AddMinutes(90)
                };
                await _faseRepository.CreateAsync(faseFinalB);
            }
        }
 
        private string DeterminarPlanProgresion(int count)
        {
            // La elección entre la variante 1 y 2 es aleatoria (ej: A1 o A2)
            string variant = new Random().Next(1, 3).ToString(); 

            if (count >= 10 && count <= 18) return $"Plan A{variant}";
            if (count >= 19 && count <= 27) return $"Plan B{variant}";
            if (count >= 28 && count <= 36) return $"Plan C{variant}";
            if (count >= 37 && count <= 45) return $"Plan D{variant}";
            if (count >= 46 && count <= 54) return $"Plan E{variant}";
            if (count >= 55 && count <= 63) return $"Plan F{variant}";
            if (count >= 64 && count <= 72) return $"Plan G{variant}";
            
            return null; // Directo a final u otra excepción
        }

        private Entidades.Entidades.Fase CrearFaseConResultados(int etapaId, string nombreFase, int numeroFase, List<Entidades.Entidades.Inscripcion> inscripcionesBase, DateTime? fechaHora = null)
        {
            var fase = new Entidades.Entidades.Fase
            {
                EtapaId = etapaId,
                NombreFase = nombreFase,
                NumeroFase = numeroFase,
                FechaHoraProgramada = fechaHora,
                Estado = "Programada"
            };

            var availableLanes = Enumerable.Range(1, 9).ToList();
            var rng = new Random();

            // 1. Asignar primero a los Cabezas de Serie (prioridad carril 5, luego 4 y 6)
            foreach (var insc in inscripcionesBase.Where(i => i.EsCabezaDeSerie).ToList())
            {
                int carrilAsignado = 0;
                if (availableLanes.Contains(5)) carrilAsignado = 5;
                else if (availableLanes.Contains(4)) carrilAsignado = 4;
                else if (availableLanes.Contains(6)) carrilAsignado = 6;
                else if (availableLanes.Any())
                {
                    int indexItem = rng.Next(availableLanes.Count);
                    carrilAsignado = availableLanes[indexItem];
                }

                if (carrilAsignado > 0)
                {
                    availableLanes.Remove(carrilAsignado);
                    fase.Resultados.Add(new Entidades.Entidades.Resultado
                    {
                        InscripcionId = insc.Id,
                        Carril = carrilAsignado,
                        Estado = SportTrack_v1.Entidades.Enums.EstadoResultadoEnum.Pendiente
                    });
                }
            }

            // 2. Asignar al resto de forma aleatoria
            foreach (var insc in inscripcionesBase.Where(i => !i.EsCabezaDeSerie).ToList())
            {
                int carrilAsignado = 0;
                if (availableLanes.Any())
                {
                    int indexItem = rng.Next(availableLanes.Count);
                    carrilAsignado = availableLanes[indexItem];
                    availableLanes.RemoveAt(indexItem);
                }

                fase.Resultados.Add(new Entidades.Entidades.Resultado
                {
                    InscripcionId = insc.Id,
                    Carril = carrilAsignado > 0 ? carrilAsignado : null,
                    Estado = SportTrack_v1.Entidades.Enums.EstadoResultadoEnum.Pendiente
                });
            }

            return fase;
        }

        public async Task<IEnumerable<FaseDto>> PromoverFasesAsync(int eventoPruebaId)
        {
            // 1. Obtener todas las fases con sus resultados e inscripciones (usando el repo de fases que es más completo)
            var todasLasFases = (await _faseRepository.GetByEventoPruebaIdAsync(eventoPruebaId)).ToList();
            if (!todasLasFases.Any()) return new List<FaseDto>();

            // Reconstruir la lista de etapas a partir de las fases para asegurar integridad de datos cargados
            var etapas = todasLasFases.GroupBy(f => f.EtapaId)
                                      .Select(g => g.First().Etapa)
                                      .OrderBy(e => e.Orden)
                                      .ToList();

            // 2. Encontrar la etapa más alta que tenga resultados (tiempo o posición)
            var etapaCandidata = etapas.OrderByDescending(e => e.Orden)
                                       .Where(e => e.Tipo != SportTrack_v1.Entidades.Enums.TipoEtapaEnum.Final)
                                       .FirstOrDefault(e => {
                                           var fasesDeEsaEtapa = todasLasFases.Where(f => f.EtapaId == e.Id);
                                           return fasesDeEsaEtapa.Any(f => f.Resultados.Any(r => r.TiempoOficial.HasValue || r.Posicion.HasValue));
                                       });

            if (etapaCandidata == null)
            {
                etapaCandidata = etapas.OrderBy(e => e.Orden)
                                       .Where(e => e.Tipo != SportTrack_v1.Entidades.Enums.TipoEtapaEnum.Final)
                                       .FirstOrDefault();
            }

            if (etapaCandidata == null)
            {
                throw new InvalidOperationException("No se encontró ninguna etapa con fases para promover.");
            }

            var etapaActual = etapaCandidata;

            // 3. Verificar si está completa usando la lista plana de fases
            var fasesDeLaEtapa = todasLasFases.Where(f => f.EtapaId == etapaActual.Id).ToList();
            var fasesIncompletas = fasesDeLaEtapa
                .Where(f => !f.Resultados.Any() || !f.Resultados.Any(r => r.TiempoOficial.HasValue || r.Posicion.HasValue))
                .Select(f => f.NombreFase)
                .ToList();

            if (fasesIncompletas.Any())
            {
                string listaFases = string.Join(", ", fasesIncompletas);
                throw new InvalidOperationException($"No se puede promover la etapa '{etapaActual.Nombre}' porque faltan resultados en: {listaFases}. Asegúrate de cargar y GUARDAR los tiempos de todas las series.");
            }

            // 2. Borrar etapas de orden superior (futuras) SOLO SI no tienen resultados ya cargados.
            var etapasAEliminar = etapas
                .Where(e => e.Orden > etapaActual.Orden)
                .Where(e => !e.Fases.Any(f => f.Resultados.Any(r => r.TiempoOficial.HasValue)))
                .ToList();

            foreach(var e in etapasAEliminar) await _etapaRepository.DeleteAsync(e.Id); 
            
            // Re-obtener fases y etapas para tener la lista fresca después del borrado
            todasLasFases = (await _faseRepository.GetByEventoPruebaIdAsync(eventoPruebaId)).ToList();
            etapas = todasLasFases.GroupBy(f => f.EtapaId).Select(g => g.First().Etapa).OrderBy(e => e.Orden).ToList();
            etapaActual = etapas.First(e => e.Id == etapaActual.Id);
            fasesDeLaEtapa = todasLasFases.Where(f => f.EtapaId == etapaActual.Id).ToList();

            // 3. Obtener resultados de la etapa actual
            var resultadosEtapa = fasesDeLaEtapa.SelectMany(f => f.Resultados)
                                    .Where(r => r.TiempoOficial.HasValue)
                                    .ToList();

            if (!resultadosEtapa.Any()) return await GetFasesPorEventoPruebaAsync(eventoPruebaId);

            // Determinar horario de inicio de la siguiente etapa (40m después de la última fase de la etapa actual)
            var lastFaseTime = fasesDeLaEtapa.Max(f => f.FechaHoraProgramada) ?? DateTime.UtcNow;
            DateTime nextTime = lastFaseTime.AddMinutes(40);

            var finalistsA = new List<Entidades.Entidades.Inscripcion>();
            var finalistsB = new List<Entidades.Entidades.Inscripcion>();
            var finalistsC = new List<Entidades.Entidades.Inscripcion>();
            var nextSemis = new List<Entidades.Entidades.Inscripcion>();

            var phasesRanked = fasesDeLaEtapa
                                .Select(f => f.Resultados
                                    .Where(r => r.TiempoOficial.HasValue)
                                    .OrderBy(r => r.TiempoOficial!.Value)
                                    .Select(r => r.Inscripcion!)
                                    .ToList())
                                .ToList();

            int numHeats = phasesRanked.Count;

            if (etapaActual.Tipo == SportTrack_v1.Entidades.Enums.TipoEtapaEnum.Eliminatoria)
            {
                if (numHeats == 1) 
                {
                    finalistsA.AddRange(phasesRanked[0].Take(9));
                }
                else if (numHeats == 2)
                {
                    // 1-3 direct to Final A
                    foreach (var s in phasesRanked) finalistsA.AddRange(s.Take(3));
                    // 4-7 to SF
                    foreach (var s in phasesRanked) nextSemis.AddRange(s.Skip(3).Take(4));
                    // + 1x8th BT to SF
                    var eighths = phasesRanked
                        .Select(s => s.Count >= 8 ? s[7] : null)
                        .Where(i => i != null)
                        .Select(i => new { Insc = i!, Tiempo = etapaActual.Fases.SelectMany(f => f.Resultados).First(r => r.InscripcionId == i!.Id).TiempoOficial })
                        .OrderBy(x => x.Tiempo)
                        .ToList();
                    if (eighths.Any()) nextSemis.Add(eighths[0].Insc);
                }
                else if (numHeats == 3)
                {
                    // 1st direct to Final A
                    foreach (var s in phasesRanked) finalistsA.Add(s.First());
                    // 2-7 to SF (18 total)
                    foreach (var s in phasesRanked) nextSemis.AddRange(s.Skip(1).Take(6));
                }
                else if (numHeats == 4)
                {
                    // 1-6 + 3x7th BT to SF (27 total)
                    foreach (var s in phasesRanked) nextSemis.AddRange(s.Take(6));
                    var sevenths = phasesRanked
                        .Select(s => s.Count >= 7 ? s[6] : null)
                        .Where(i => i != null)
                        .Select(i => new { Insc = i!, Tiempo = etapaActual.Fases.SelectMany(f => f.Resultados).First(r => r.InscripcionId == i!.Id).TiempoOficial })
                        .OrderBy(x => x.Tiempo)
                        .ToList();
                    nextSemis.AddRange(sevenths.Take(3).Select(x => x.Insc));
                }
                else if (numHeats == 5)
                {
                    // 1-5 + 2x6th BT to SF (27 total)
                    foreach (var s in phasesRanked) nextSemis.AddRange(s.Take(5));
                    var sixths = phasesRanked
                        .Select(s => s.Count >= 6 ? s[5] : null)
                        .Where(i => i != null)
                        .Select(i => new { Insc = i!, Tiempo = etapaActual.Fases.SelectMany(f => f.Resultados).First(r => r.InscripcionId == i!.Id).TiempoOficial })
                        .OrderBy(x => x.Tiempo)
                        .ToList();
                    nextSemis.AddRange(sixths.Take(2).Select(x => x.Insc));
                }
            }
            else if (etapaActual.Tipo == SportTrack_v1.Entidades.Enums.TipoEtapaEnum.Semifinal)
            {
                // 1. Recuperar finalistas directos de Eliminatorias para sumarlos a la Final A
                var etapaE = etapas.FirstOrDefault(e => e.Tipo == SportTrack_v1.Entidades.Enums.TipoEtapaEnum.Eliminatoria);
                if (etapaE != null)
                {
                    // Obtener TODAS las fases de esa etapa de eliminatorias
                    var todasFasesPrueba = await _faseRepository.GetByEventoPruebaIdAsync(eventoPruebaId);
                    var fasesElim = todasFasesPrueba
                                    .Where(f => f.EtapaId == etapaE.Id)
                                    .OrderBy(f => f.NumeroFase)
                                    .ToList();

                    List<List<Entidades.Entidades.Inscripcion>> elimRanked = fasesElim
                        .Select(f => f.Resultados
                            .Where(r => r.TiempoOficial.HasValue)
                            .OrderBy(r => r.TiempoOficial!.Value)
                            .Select(r => r.Inscripcion!)
                            .ToList())
                        .ToList();
                    
                    int countElim = elimRanked.Count;
                    if (countElim == 2) {
                        // 1-3 direct to Final A (6 total)
                        foreach (var s in elimRanked) finalistsA.AddRange(s.Take(3));
                    }
                    else if (countElim == 3) {
                        // 1st direct to Final A (3 total)
                        foreach (var s in elimRanked) finalistsA.Add(s.First());
                    }
                }

                // 2. Clasificación desde las Semifinales actuales
                if (numHeats == 1) // Caso 2 Heats -> 1 Semi
                {
                    // 1-3 to Final A
                    finalistsA.AddRange(phasesRanked[0].Take(3));
                }
                else if (numHeats == 2) // Caso 3 Heats -> 2 Semis
                {
                    // 1-3 to Final A
                    foreach (var s in phasesRanked) finalistsA.AddRange(s.Take(3));
                    // 4-7 + 1x8th BT to Final B
                    foreach (var s in phasesRanked) finalistsB.AddRange(s.Skip(3).Take(4));
                    
                    var eighths = phasesRanked
                        .Select(s => s.Count >= 8 ? s[7] : null)
                        .Where(i => i != null)
                        .Select(i => new { Insc = i!, Tiempo = etapaActual.Fases.SelectMany(f => f.Resultados).First(r => r.InscripcionId == i!.Id).TiempoOficial })
                        .OrderBy(x => x.Tiempo)
                        .ToList();
                    if (eighths.Count > 0) finalistsB.Add(eighths[0].Insc);
                }
                else if (numHeats == 3) // Caso 4 o 5 Heats -> 3 Semis
                {
                    // 1-3 to Final A
                    foreach (var s in phasesRanked) finalistsA.AddRange(s.Take(3));
                    // 4-6 to Final B
                    foreach (var s in phasesRanked) finalistsB.AddRange(s.Skip(3).Take(3));
                    
                    // Si venimos de 5 Heats, hay Final C
                    if (etapaE != null)
                    {
                        var todasFases = await _faseRepository.GetByEventoPruebaIdAsync(eventoPruebaId);
                        int numElims = todasFases.Count(f => f.EtapaId == etapaE.Id);
                        if (numElims == 5)
                        {
                            // 7-9 to Final C
                            foreach (var s in phasesRanked) finalistsC.AddRange(s.Skip(6).Take(3));
                        }
                    }
                }
            }

            // CREAR SIGUIENTE ETAPA(S) - Solo si no existen ya
            DateTime tempNextTime = nextTime;

            if (nextSemis.Any() && !etapas.Any(e => e.Tipo == SportTrack_v1.Entidades.Enums.TipoEtapaEnum.Semifinal && e.Orden > etapaActual.Orden))
            {
                var etapaSemi = new Etapa { EventoPruebaId = eventoPruebaId, Nombre = "Semifinales", Tipo = SportTrack_v1.Entidades.Enums.TipoEtapaEnum.Semifinal, Orden = etapaActual.Orden + 1 };
                await _etapaRepository.CreateAsync(etapaSemi);
                
                int numNextSemis = (int)Math.Ceiling(nextSemis.Count / 9.0);
                for(int i=0; i<numNextSemis; i++) {
                    var batch = nextSemis.Where((res, index) => index % numNextSemis == i).ToList();
                    await _faseRepository.CreateAsync(CrearFaseConResultados(etapaSemi.Id, $"Semifinal {i+1}", i+1, batch, tempNextTime));
                    tempNextTime = tempNextTime.AddMinutes(10); 
                }

                tempNextTime = tempNextTime.AddMinutes(30); 
            }

            if (finalistsA.Any() || finalistsB.Any())
            {
                // Buscar si ya existe la etapa de Finales
                var etapaFinal = etapas.FirstOrDefault(e => e.Tipo == SportTrack_v1.Entidades.Enums.TipoEtapaEnum.Final && e.Orden > etapaActual.Orden);
                
                if (etapaFinal == null)
                {
                    int ordenFinal = etapaActual.Orden + (nextSemis.Any() ? 2 : 1);
                    etapaFinal = new Etapa { EventoPruebaId = eventoPruebaId, Nombre = "Finales", Tipo = SportTrack_v1.Entidades.Enums.TipoEtapaEnum.Final, Orden = ordenFinal };
                    await _etapaRepository.CreateAsync(etapaFinal);
                }

                // Procesar Final A
                if (finalistsA.Any()) 
                {
                    // Buscar en TODAS las fases de la prueba para borrar cualquier Final A previa (incompleta)
                    var faseFinalA = todasLasFases.FirstOrDefault(f => f.EtapaId == etapaFinal.Id && f.NombreFase == "Final A");
                    if (faseFinalA != null) await _faseRepository.DeleteAsync(faseFinalA.Id);
                    
                    await _faseRepository.CreateAsync(CrearFaseConResultados(etapaFinal.Id, "Final A", 1, finalistsA, tempNextTime));
                    tempNextTime = tempNextTime.AddMinutes(10);
                }
                
                // Procesar Final B
                if (finalistsB.Any()) 
                {
                    var faseFinalB = todasLasFases.FirstOrDefault(f => f.EtapaId == etapaFinal.Id && f.NombreFase == "Final B");
                    if (faseFinalB != null) await _faseRepository.DeleteAsync(faseFinalB.Id);
                    
                    await _faseRepository.CreateAsync(CrearFaseConResultados(etapaFinal.Id, "Final B", 2, finalistsB, tempNextTime));
                    tempNextTime = tempNextTime.AddMinutes(10);
                }

                // Procesar Final C
                if (finalistsC.Any()) 
                {
                    var faseFinalC = todasLasFases.FirstOrDefault(f => f.EtapaId == etapaFinal.Id && f.NombreFase == "Final C");
                    if (faseFinalC != null) await _faseRepository.DeleteAsync(faseFinalC.Id);
                    
                    await _faseRepository.CreateAsync(CrearFaseConResultados(etapaFinal.Id, "Final C", 3, finalistsC, tempNextTime));
                }
            }

            // Auditoria
            await _auditService.RegistrarAccionAsync("PROMOTE_STAGE", 
                $"Promoción de etapa ejecutada para la Prueba ID {eventoPruebaId}. Etapa actual: {etapaActual.Nombre}", null, "Competencia");

            return await GetFasesPorEventoPruebaAsync(eventoPruebaId);
        }
        public async Task<FaseDto> IniciarFaseAsync(int id, DateTime? manualStartTime = null)
        {
            var fase = await _faseRepository.GetByIdAsync(id);
            if (fase == null) throw new KeyNotFoundException("Fase no encontrada");

            // Si el cliente nos manda su hora sincronizada (para evitar latencia de red), la usamos.
            // Si no, usamos la hora actual del servidor.
            fase.FechaHoraInicioReal = manualStartTime ?? DateTime.UtcNow;
            fase.Estado = "En Carrera";
            await _faseRepository.UpdateAsync(fase);

            // Auditoria
            await _auditService.RegistrarAccionAsync("START_RACE", 
                $"Carrera iniciada: {fase.NombreFase} (ID: {id}) a las {fase.FechaHoraInicioReal}", null, "Competencia");

            // Notificar por SignalR
            await _hubContext.Clients.Group($"race_{id}").SendAsync("RaceStarted", id, fase.FechaHoraInicioReal);
            
            // Notificación Global (para usuarios fuera de la regata específica, como el Cronometrista en su Dashboard)
            await _hubContext.Clients.All.SendAsync("globalRaceStarted", id, fase.FechaHoraInicioReal);

            return _mapper.Map<FaseDto>(fase);
        }

        public async Task<FaseDto> FinalizarFaseAsync(int id)
        {
            var fase = await _faseRepository.GetByIdAsync(id);
            if (fase == null) throw new KeyNotFoundException("Fase no encontrada");

            fase.Estado = "Finalizada";
            fase.FechaHoraFinReal = DateTime.UtcNow;

            // Al finalizar oficialmente, marcamos todos los resultados como Oficiales
            if (fase.Resultados != null && fase.Resultados.Any())
            {
                // Ordenamos por tiempo para asignar posiciones automáticamente
                var conTiempo = fase.Resultados
                                    .Where(r => r.TiempoOficial != null)
                                    .OrderBy(r => r.TiempoOficial)
                                    .ToList();

                int pos = 1;
                foreach (var res in conTiempo)
                {
                    res.Estado = SportTrack_v1.Entidades.Enums.EstadoResultadoEnum.Oficial;
                    res.Posicion = pos++;
                }
            }

            await _faseRepository.UpdateAsync(fase);

            // Auditoria
            await _auditService.RegistrarAccionAsync("FINISH_RACE", 
                $"Carrera oficializada: {fase.NombreFase} (ID: {id})", null, "Competencia");

            // Notificar por SignalR (Local y Global)
            await _hubContext.Clients.Group($"race_{id}").SendAsync("RaceFinished", id);
            await _hubContext.Clients.All.SendAsync("globalRaceOfficialized", id);

            return _mapper.Map<FaseDto>(fase);
        }

        public async Task<bool> DeleteFaseAsync(int id)
        {
            await _faseRepository.DeleteAsync(id);
            return true;
        }

        public async Task<FaseDto> ReiniciarFaseAsync(int id)
        {
            var fase = await _faseRepository.GetByIdAsync(id);
            if (fase == null) throw new KeyNotFoundException("Fase no encontrada");

            // 1. Limpiar tiempos de la fase
            fase.FechaHoraInicioReal = null;
            fase.FechaHoraFinReal = null;
            fase.Estado = "Programada";

            // 2. Limpiar resultados de cada carril pero conservar la inscripción y el carril
            if (fase.Resultados != null)
            {
                foreach (var res in fase.Resultados)
                {
                    res.TiempoOficial = null;
                    res.Posicion = null;
                    res.Estado = SportTrack_v1.Entidades.Enums.EstadoResultadoEnum.Pendiente;
                }
            }

            await _faseRepository.UpdateAsync(fase);

            // Auditoria
            await _auditService.RegistrarAccionAsync("RESET_RACE", 
                $"Carrera reiniciada: {fase.NombreFase} (ID: {id}). Se limpiaron los tiempos.", null, "Competencia");

            // 3. Notificar a los clientes SignalR que la carrera se reinició
            await _hubContext.Clients.Group($"race_{id}").SendAsync("RaceReset", id);

            return _mapper.Map<FaseDto>(fase);
        }

        public async Task<FaseDto> EnviarARevisionAsync(int id)
        {
            var fase = await _faseRepository.GetByIdAsync(id);
            if (fase == null) throw new KeyNotFoundException("Fase no encontrada");

            fase.Estado = "Pendiente de Validación";
            fase.FechaHoraFinReal = DateTime.UtcNow;

            // Al enviar a revisión, marcamos los resultados como Preliminares
            if (fase.Resultados != null)
            {
                foreach (var res in fase.Resultados)
                {
                    if (res.TiempoOficial != null)
                    {
                        res.Estado = SportTrack_v1.Entidades.Enums.EstadoResultadoEnum.Preliminar;
                    }
                }
            }

            await _faseRepository.UpdateAsync(fase);
            
            // Auditoria
            await _auditService.RegistrarAccionAsync("REVIEW_RACE", 
                $"Carrera enviada a revisión: {fase.NombreFase} (ID: {id})", null, "Competencia");

            Console.WriteLine($"[SignalR-Debug] Emitting GlobalRaceInReview for Fase {fase.Id}: {fase.NombreFase}");

            // Notificar que está en revisión (Local a la carrera y Global para el Juez)
            await _hubContext.Clients.Group($"race_{id}").SendAsync("RaceInReview", id);
            await _hubContext.Clients.All.SendAsync("globalRaceInReview", new { id = fase.Id, nombre = fase.NombreFase });

            return _mapper.Map<FaseDto>(fase);
        }

        public async Task<IEnumerable<FaseDto>> GetFasesPorEventoAsync(int eventoId)
        {
            var fases = await _faseRepository.GetByEventoIdAsync(eventoId);
            return _mapper.Map<IEnumerable<FaseDto>>(fases);
        }

        public async Task BatchUpdateFasesAsync(List<FaseBatchUpdateDto> dto)
        {
            foreach (var item in dto)
            {
                var fase = await _faseRepository.GetByIdAsync(item.Id);
                if (fase != null)
                {
                    // Si el Kind es Utc, lo dejamos como está. 
                    // Si es Unspecified (viene del string sin Z), lo tratamos como UTC para no romper la lógica de la BD,
                    // pero lo ideal es que el frontend mande el ISO con Z (como acabamos de corregir).
                    fase.FechaHoraProgramada = item.FechaHoraProgramada.Kind == DateTimeKind.Utc 
                        ? item.FechaHoraProgramada 
                        : DateTime.SpecifyKind(item.FechaHoraProgramada, DateTimeKind.Utc);
                    await _faseRepository.UpdateAsync(fase);
                }
            }
        }

        private DateTime GetUtcTime(DateTime localDateTime, string timeZoneId)
        {
            try
            {
                // Intentar búsqueda estándar
                var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                var unspecified = DateTime.SpecifyKind(localDateTime, DateTimeKind.Unspecified);
                return TimeZoneInfo.ConvertTimeToUtc(unspecified, tz);
            }
            catch
            {
                // Fallback robusto para Argentina (UTC-3) si el servidor no reconoce el TimeZoneId
                if (timeZoneId.Contains("Argentina") || timeZoneId.Contains("Buenos_Aires"))
                {
                    return DateTime.SpecifyKind(localDateTime.AddHours(3), DateTimeKind.Utc);
                }
                // Si todo falla, asumimos que ya es UTC para evitar desvíos indeterminados
                return DateTime.SpecifyKind(localDateTime, DateTimeKind.Utc);
            }
        }

        public async Task<IEnumerable<FaseDto>> GenerarFasesManualAsync(int eventoPruebaId, List<ManualPlacementDto> placements)
        {
            if (placements == null || !placements.Any())
                throw new ArgumentException("Debe proporcionar al menos una ubicación para generar las fases.");

            // Validar que no haya carriles duplicados dentro de la misma serie
            var agrupadoPorSerieCheck = placements.GroupBy(p => p.Serie);
            foreach (var grupo in agrupadoPorSerieCheck)
            {
                var carrilesRepetidos = grupo.GroupBy(p => p.Carril)
                                            .Where(g => g.Count() > 1)
                                            .Select(g => g.Key)
                                            .ToList();
                if (carrilesRepetidos.Any())
                {
                    throw new ArgumentException($"El carril {carrilesRepetidos.First()} está repetido en la Serie {grupo.Key}.");
                }
            }

            // 1. LIMPIEZA TOTAL
            await _etapaRepository.DeleteByEventoPruebaIdAsync(eventoPruebaId);

            var ep = await _eventoRepository.GetEventoPruebaByIdAsync(eventoPruebaId);
            DateTime nextTime;
            if (ep?.FechaHora != null)
            {
                nextTime = ep.FechaHora;
            }
            else
            {
                var baseDate = ep?.Evento?.Fecha.Date ?? DateTime.UtcNow.Date;
                var horaBase = ep?.Evento?.HoraInicioEvento ?? new TimeSpan(8, 0, 0);
                try
                {
                    var tzId = ep?.Evento?.TimeZoneId ?? "America/Argentina/Buenos_Aires";
                    var tz = TimeZoneInfo.FindSystemTimeZoneById(tzId);
                    var localDateTime = DateTime.SpecifyKind(baseDate.Add(horaBase), DateTimeKind.Unspecified);
                    nextTime = TimeZoneInfo.ConvertTimeToUtc(localDateTime, tz);
                }
                catch
                {
                    nextTime = DateTime.SpecifyKind(baseDate.Add(horaBase), DateTimeKind.Utc);
                }
            }

            // Asignar Plan de Progresión Automáticamente
            if (ep != null)
            {
                int inscriptosCount = placements.Count;
                ep.PlanProgresionAsignado = DeterminarPlanProgresion(inscriptosCount);
                await _eventoRepository.UpdateEventoPruebaAsync(ep);
            }

            // Determinar cuántas series hay
            var numSeries = placements.Max(p => p.Serie);

            // Crear Etapa de Eliminatorias (o Finales si es solo 1 serie)
            var etapaElim = new Etapa
            {
                EventoPruebaId = eventoPruebaId,
                Nombre = numSeries <= 1 ? "Finales" : "Eliminatorias",
                Tipo = numSeries <= 1 ? SportTrack_v1.Entidades.Enums.TipoEtapaEnum.Final : SportTrack_v1.Entidades.Enums.TipoEtapaEnum.Eliminatoria,
                Orden = numSeries <= 1 ? 3 : 1
            };
            await _etapaRepository.CreateAsync(etapaElim);

            // Agrupar placements por serie
            var agrupadoPorSerie = placements.GroupBy(p => p.Serie).OrderBy(g => g.Key);

            foreach (var grupo in agrupadoPorSerie)
            {
                var nroSerie = grupo.Key;
                var fase = new Entidades.Entidades.Fase
                {
                    EtapaId = etapaElim.Id,
                    NombreFase = numSeries <= 1 ? "Final A" : $"Serie {nroSerie}",
                    NumeroFase = nroSerie,
                    Estado = "Programada",
                    FechaHoraProgramada = nextTime
                };

                foreach (var p in grupo)
                {
                    fase.Resultados.Add(new Entidades.Entidades.Resultado
                    {
                        InscripcionId = p.InscripcionId,
                        Carril = p.Carril,
                        Estado = SportTrack_v1.Entidades.Enums.EstadoResultadoEnum.Pendiente
                    });
                }

                await _faseRepository.CreateAsync(fase);
                nextTime = nextTime.AddMinutes(10);
            }

            // Si hay más de una serie, pre-generamos las siguientes etapas vacías (SF/Finales) como en el modo auto
            if (numSeries > 1)
            {
                var inscriptosCount = placements.Count;
                await PreGenerarSiguientesEtapasAsync(eventoPruebaId, inscriptosCount, numSeries, nextTime);
            }

            return await GetFasesPorEventoPruebaAsync(eventoPruebaId);
        }

        public async Task UpdateResultadoStatusAsync(int resultadoId, string status)
        {
            var res = await _faseRepository.GetResultadoByIdAsync(resultadoId);
            if (res == null) return;

            // Mapeo de strings (DNS, DNF, DSQ) al enum
            if (Enum.TryParse<SportTrack_v1.Entidades.Enums.EstadoResultadoEnum>(status, true, out var result))
            {
                res.Estado = result;
            }
            else if (status.ToUpper() == "PENDIENTE")
            {
                res.Estado = SportTrack_v1.Entidades.Enums.EstadoResultadoEnum.Pendiente;
            }
            else if (status.ToUpper() == "DSQ")
            {
                res.Estado = SportTrack_v1.Entidades.Enums.EstadoResultadoEnum.Descalificado;
            }

            await _faseRepository.UpdateResultadoAsync(res);
            
            // Notificar Globalmente
            await _hubContext.Clients.All.SendAsync("GlobalResultStatusUpdated", resultadoId, status);
        }

    }
}
