using AutoMapper;
using SportTrack_v1.Controladores.Fase.Dtos;
using SportTrack_v1.Controladores.Inscripcion;
using SportTrack_v1.Controladores.Evento;
using SportTrack_v1.Entidades.Entidades;
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
        Task<FaseDto> IniciarFaseAsync(int id);
        Task<FaseDto> FinalizarFaseAsync(int id);
        Task<bool> DeleteFaseAsync(int id);
    }

    public class FaseService : IFaseService
    {
        private readonly IFaseRepository _faseRepository;
        private readonly IEtapaRepository _etapaRepository;
        private readonly IInscripcionRepository _inscripcionRepository;
        private readonly IEventoRepository _eventoRepository;

        private readonly Microsoft.AspNetCore.SignalR.IHubContext<SportTrack_v1.Controladores.Hubs.TimingHub> _hubContext;
        private readonly IMapper _mapper;

        public FaseService(
            IFaseRepository faseRepository, 
            IEtapaRepository etapaRepository,
            IInscripcionRepository inscripcionRepository, 
            IEventoRepository eventoRepository,
            Microsoft.AspNetCore.SignalR.IHubContext<SportTrack_v1.Controladores.Hubs.TimingHub> hubContext,
            IMapper mapper)
        {
            _faseRepository = faseRepository;
            _etapaRepository = etapaRepository;
            _inscripcionRepository = inscripcionRepository;
            _eventoRepository = eventoRepository;
            _hubContext = hubContext;

            _mapper = mapper;
        }

        public async Task<IEnumerable<FaseDto>> GetFasesPorEventoPruebaAsync(int eventoPruebaId)
        {
            var fases = await _faseRepository.GetByEventoPruebaIdAsync(eventoPruebaId);
            if (fases == null) return new List<FaseDto>();

            // Lógica para ocultar la Final si las etapas previas no han terminado
            var listFases = fases.ToList();
            if (!listFases.Any()) return new List<FaseDto>();

            var etapas = listFases.GroupBy(f => f.EtapaId)
                                  .Select(g => {
                                      var firstFase = g.FirstOrDefault();
                                      return new { 
                                          EtapaId = g.Key, 
                                          Tipo = firstFase?.Etapa?.Tipo, 
                                          Orden = firstFase?.Etapa?.Orden ?? 0,
                                          Completa = g.All(f => f.Resultados != null && f.Resultados.Any(r => r.TiempoOficial.HasValue))
                                      };
                                  })
                                  .OrderBy(e => e.Orden)
                                  .ToList();

            var fasesFiltradas = listFases.Where(f => {
                if (f.Etapa == null || f.Etapa.Tipo != SportTrack_v1.Entidades.Enums.TipoEtapaEnum.Final) return true;
                
                // Si es Final, solo mostrar si todas las etapas de orden inferior están COMPLETAS
                var etapasPrevias = etapas.Where(e => e.Orden < f.Etapa.Orden);
                return etapasPrevias.All(e => e.Completa);
            });

            return _mapper.Map<IEnumerable<FaseDto>>(fasesFiltradas);
        }

        public async Task<IEnumerable<FaseDto>> GenerarFasesAutoAsync(int eventoPruebaId)
        {
            // Verificación de seguridad: No permitir re-sortear si ya hay resultados oficiales
            var fasesExistentes = await _faseRepository.GetByEventoPruebaIdAsync(eventoPruebaId);
            if (fasesExistentes.Any(f => f.Resultados.Any(r => r.TiempoOficial.HasValue)))
            {
                throw new InvalidOperationException("No se puede volver a sortear una regata que ya tiene resultados oficiales cargados.");
            }

            // 1. Limpieza total de etapas y fases previas para esta prueba
            await _etapaRepository.DeleteByEventoPruebaIdAsync(eventoPruebaId);

            // 2. Obtener inscripciones y datos base de la prueba para horarios
            var inscripciones = await _inscripcionRepository.GetByEventoPruebaIdAsync(eventoPruebaId);
            var ep = await _eventoRepository.GetEventoPruebaByIdAsync(eventoPruebaId);
            if (!inscripciones.Any() || ep == null)
                return new List<FaseDto>();

            var inscripcionesList = inscripciones.ToList();
            int inscriptosCount = inscripcionesList.Count;
            DateTime nextTime = ep.FechaHora;

            // Determinar estructura inicial
            if (inscriptosCount <= 9)
            {
                // Unica etapa: Final
                var etapaFinal = new Etapa { 
                    EventoPruebaId = eventoPruebaId, 
                    Nombre = "Finales", 
                    Tipo = SportTrack_v1.Entidades.Enums.TipoEtapaEnum.Final, 
                    Orden = 1 
                };
                await _etapaRepository.CreateAsync(etapaFinal);

                var faseFinal = CrearFaseConResultados(etapaFinal.Id, "Final A", 1, inscripcionesList, nextTime);
                await _faseRepository.CreateAsync(faseFinal);
            }
            else
            {
                // Etapa 1: Eliminatorias
                var etapaElim = new Etapa { 
                    EventoPruebaId = eventoPruebaId, 
                    Nombre = "Eliminatorias", 
                    Tipo = SportTrack_v1.Entidades.Enums.TipoEtapaEnum.Eliminatoria, 
                    Orden = 1 
                };
                await _etapaRepository.CreateAsync(etapaElim);

                int numSeries = (int)Math.Ceiling(inscriptosCount / 9.0);
                var rng = new Random();

                // Separar cabezas de serie del resto
                var cabezas = inscripcionesList.Where(i => i.EsCabezaDeSerie).OrderBy(x => rng.Next()).ToList();
                var regulares = inscripcionesList.Where(i => !i.EsCabezaDeSerie).OrderBy(x => rng.Next()).ToList();

                // Preparar cubetas para cada serie
                var seriesBuckets = new List<List<Entidades.Entidades.Inscripcion>>();
                for (int i = 0; i < numSeries; i++) seriesBuckets.Add(new List<Entidades.Entidades.Inscripcion>());

                // Distribuir cabezas (Round Robin)
                int currentBucket = 0;
                foreach (var c in cabezas)
                {
                    seriesBuckets[currentBucket % numSeries].Add(c);
                    currentBucket++;
                }

                // Distribuir el resto
                foreach (var r in regulares)
                {
                    // Buscar el bucket con menos gente que no esté lleno (max 9)
                    var targetBucket = seriesBuckets
                                        .Where(b => b.Count < 9)
                                        .OrderBy(b => b.Count)
                                        .FirstOrDefault();
                    
                    if (targetBucket != null) targetBucket.Add(r);
                }

                // Crear las fases
                for (int i = 0; i < numSeries; i++)
                {
                    var faseSerie = CrearFaseConResultados(etapaElim.Id, $"Serie {i + 1}", i + 1, seriesBuckets[i], nextTime);
                    await _faseRepository.CreateAsync(faseSerie);
                    nextTime = nextTime.AddMinutes(10);
                }
            }


            return await GetFasesPorEventoPruebaAsync(eventoPruebaId);
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
            // Obtener etapas actuales
            var etapas = (await _etapaRepository.GetByEventoPruebaIdAsync(eventoPruebaId)).ToList();
            
            // SECUENCIA EXPLICITA DE PROMOCION
            // 1. Verificar Eliminatorias
            var etapaElim = etapas.FirstOrDefault(e => e.Tipo == SportTrack_v1.Entidades.Enums.TipoEtapaEnum.Eliminatoria);
            if (etapaElim != null && etapaElim.Fases.All(f => f.Resultados.Any(r => r.TiempoOficial.HasValue)))
            {
                // Solo procedemos si la siguiente etapa (Semis o Final) está incompleta o no existe.
                // En este caso, el motor siempre recalculará desde la etapa más baja que esté completa.
            }

            // Buscamos la etapa que queremos procesar: la etapa más alta que esté completa pero que su promoción no esté terminada 
            // o simplemente forzamos la promoción de la etapa que estamos viendo.
            // Para ser robustos, si Semis está completa, promovemos Semis. Si no, si Heats está completa, promovemos Heats.
            
            var etapaASimular = etapas.OrderByDescending(e => e.Orden)
                                      .Where(e => e.Tipo != SportTrack_v1.Entidades.Enums.TipoEtapaEnum.Final)
                                      .FirstOrDefault(e => e.Fases.All(f => f.Resultados.Any(r => r.TiempoOficial.HasValue)));

            if (etapaASimular == null)
            {
                return await GetFasesPorEventoPruebaAsync(eventoPruebaId);
            }

            var etapaActual = etapaASimular;

            // Verificación de integridad: ¿Están todas las fases de la etapa actual ya con resultados?
            // Si falta alguna fase por cargar, NO promovemos todavía para evitar grillas incompletas.
            bool etapaCompleta = etapaActual.Fases.All(f => f.Resultados.Any(r => r.TiempoOficial.HasValue));
            if (!etapaCompleta)
            {
                return await GetFasesPorEventoPruebaAsync(eventoPruebaId);
            }

            // 2. Borrar etapas de orden superior (futuras) SOLO SI no tienen resultados ya cargados.
            var etapasAEliminar = etapas
                .Where(e => e.Orden > etapaActual.Orden)
                .Where(e => !e.Fases.Any(f => f.Resultados.Any(r => r.TiempoOficial.HasValue)))
                .ToList();

            foreach(var e in etapasAEliminar) await _etapaRepository.DeleteAsync(e.Id); 
            
            // Re-obtener etapas para tener la lista fresca después del borrado
            etapas = (await _etapaRepository.GetByEventoPruebaIdAsync(eventoPruebaId)).ToList();

            // 3. Obtener resultados de la etapa actual
            var resultadosEtapa = etapaActual.Fases.SelectMany(f => f.Resultados)
                                    .Where(r => r.TiempoOficial.HasValue)
                                    .ToList();

            if (!resultadosEtapa.Any()) return await GetFasesPorEventoPruebaAsync(eventoPruebaId);

            // Determinar horario de inicio de la siguiente etapa (40m después de la última fase de la etapa actual)
            var lastFaseTime = etapaActual.Fases.Max(f => f.FechaHoraProgramada) ?? DateTime.Now;
            DateTime nextTime = lastFaseTime.AddMinutes(40);

            // LOGICA DE PROMOSION BASADA EN TIPO DE ETAPA Y NUMERO DE FASES
            var finalistsA = new List<Entidades.Entidades.Inscripcion>();
            var finalistsB = new List<Entidades.Entidades.Inscripcion>();
            var nextSemis = new List<Entidades.Entidades.Inscripcion>();

            var phasesRanked = etapaActual.Fases
                                .Select(f => f.Resultados
                                    .Where(r => r.TiempoOficial.HasValue)
                                    .OrderBy(r => r.TiempoOficial!.Value)
                                    .Select(r => r.Inscripcion!)
                                    .ToList())
                                .ToList();

            int numPhases = phasesRanked.Count;

            if (etapaActual.Tipo == SportTrack_v1.Entidades.Enums.TipoEtapaEnum.Eliminatoria)
            {
                if (numPhases == 1) 
                {
                    finalistsA.AddRange(phasesRanked[0].Take(9));
                }
                else if (numPhases == 2)
                {
                    // 1st-3rd direct to Final A
                    foreach (var s in phasesRanked) finalistsA.AddRange(s.Take(3));
                    
                    // 4th-7th to SF
                    foreach (var s in phasesRanked) nextSemis.AddRange(s.Skip(3).Take(4));
                    
                    // + 1x8th BT (Best Time) to SF
                    var eighths = phasesRanked
                        .Select(s => s.Count >= 8 ? s[7] : null)
                        .Where(i => i != null)
                        .Select(i => new { Insc = i!, Tiempo = etapaActual.Fases.SelectMany(f => f.Resultados).First(r => r.InscripcionId == i!.Id).TiempoOficial })
                        .OrderBy(x => x.Tiempo)
                        .ToList();
                    if (eighths.Any()) nextSemis.Add(eighths[0].Insc);
                }
                else if (numPhases == 3)
                {
                    // 1st to Final A
                    foreach (var s in phasesRanked) finalistsA.Add(s.First());
                    // 2nd-7th to SF (18 total)
                    foreach (var s in phasesRanked) nextSemis.AddRange(s.Skip(1).Take(6));
                }
                else if (numPhases == 4)
                {
                    // 1st-6th + 3x7th BT to SF
                    foreach (var s in phasesRanked) nextSemis.AddRange(s.Take(6));
                    var sevenths = phasesRanked
                        .Select(s => s.Count >= 7 ? s[6] : null)
                        .Where(i => i != null)
                        .Select(i => new { Insc = i!, Tiempo = etapaActual.Fases.SelectMany(f => f.Resultados).First(r => r.InscripcionId == i!.Id).TiempoOficial })
                        .OrderBy(x => x.Tiempo)
                        .ToList();
                    nextSemis.AddRange(sevenths.Take(3).Select(x => x.Insc));
                }
                else if (numPhases == 5)
                {
                    // 1st-5th + 2x6th BT to SF
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
                // 1. Recuperar finalistas directos de Eliminatorias (si existen)
                etapaElim = etapas.FirstOrDefault(e => e.Tipo == SportTrack_v1.Entidades.Enums.TipoEtapaEnum.Eliminatoria);
                if (etapaElim != null)
                {
                    var elimRanked = etapaElim.Fases
                        .Select(f => f.Resultados
                            .Where(r => r.TiempoOficial.HasValue)
                            .OrderBy(r => r.TiempoOficial!.Value)
                            .Select(r => r.Inscripcion!)
                            .ToList())
                        .ToList();
                    
                    if (elimRanked.Count == 2) {
                        foreach (var s in elimRanked) finalistsA.AddRange(s.Take(3));
                    }
                    else if (elimRanked.Count == 3) {
                        foreach (var s in elimRanked) finalistsA.Add(s.First());
                    }
                }

                // 2. Agregar ganadores de Semifinal segun reglas oficiales
                if (numPhases == 1)
                {
                    // 1-3 to Final A (rest out)
                    finalistsA.AddRange(phasesRanked[0].Take(3));
                }
                else if (numPhases == 2)
                {
                    // 1-3 each to Final A
                    foreach (var s in phasesRanked) finalistsA.AddRange(s.Take(3));
                    // Nota: El usuario especificó que para 2 semis NO hay Final B.
                }
                else if (numPhases == 3)
                {
                    // 1-3 each to Final A
                    foreach (var s in phasesRanked) finalistsA.AddRange(s.Take(3));
                    // 4-6 each to Final B
                    foreach (var s in phasesRanked) finalistsB.AddRange(s.Skip(3).Take(3));
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
                    var faseFinalA = etapaFinal.Fases?.FirstOrDefault(f => f.NombreFase == "Final A");
                    if (faseFinalA != null) await _faseRepository.DeleteAsync(faseFinalA.Id); // Limpiar para recrear con los 9 definitivos
                    
                    await _faseRepository.CreateAsync(CrearFaseConResultados(etapaFinal.Id, "Final A", 1, finalistsA, tempNextTime));
                    tempNextTime = tempNextTime.AddMinutes(10);
                }
                
                // Procesar Final B - SOLO SI HAY FINALISTAS B
                var faseFinalB_Existente = etapaFinal.Fases?.FirstOrDefault(f => f.NombreFase == "Final B");
                if (finalistsB.Any()) 
                {
                    if (faseFinalB_Existente != null) await _faseRepository.DeleteAsync(faseFinalB_Existente.Id);
                    await _faseRepository.CreateAsync(CrearFaseConResultados(etapaFinal.Id, "Final B", 2, finalistsB, tempNextTime));
                }
                else if (faseFinalB_Existente != null)
                {
                    // Si ya no debe haber Final B, la eliminamos
                    await _faseRepository.DeleteAsync(faseFinalB_Existente.Id);
                }
            }


            return await GetFasesPorEventoPruebaAsync(eventoPruebaId);
        }
        public async Task<FaseDto> IniciarFaseAsync(int id)
        {
            var fase = await _faseRepository.GetByIdAsync(id);
            if (fase == null) throw new KeyNotFoundException("Fase no encontrada");

            fase.FechaHoraInicioReal = DateTime.UtcNow;
            fase.Estado = "En Carrera";
            await _faseRepository.UpdateAsync(fase);

            // Notificar por SignalR
            await _hubContext.Clients.Group($"race_{id}").SendAsync("RaceStarted", id, fase.FechaHoraInicioReal);

            return _mapper.Map<FaseDto>(fase);
        }

        public async Task<FaseDto> FinalizarFaseAsync(int id)
        {
            var fase = await _faseRepository.GetByIdAsync(id);
            if (fase == null) throw new KeyNotFoundException("Fase no encontrada");

            fase.FechaHoraFinReal = DateTime.UtcNow;
            fase.Estado = "Finalizada";
            await _faseRepository.UpdateAsync(fase);

            // Notificar por SignalR
            await _hubContext.Clients.Group($"race_{id}").SendAsync("RaceFinished", id);

            return _mapper.Map<FaseDto>(fase);
        }

        public async Task<bool> DeleteFaseAsync(int id)
        {
            await _faseRepository.DeleteAsync(id);
            return true;
        }
    }
}
