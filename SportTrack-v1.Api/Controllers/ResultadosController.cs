using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportTrack_v1.Controladores.Fase.Dtos;
using SportTrack_v1.Controladores.Resultado;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportTrack_v1.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResultadosController : ControllerBase
    {
        private readonly IResultadoRepository _resultadoRepository;
        private readonly SportTrack_v1.Api.Services.INotificadorResultados _notificador;
        private readonly IMapper _mapper;

        public ResultadosController(
            IResultadoRepository resultadoRepository, 
            SportTrack_v1.Api.Services.INotificadorResultados notificador,
            IMapper mapper)
        {
            _resultadoRepository = resultadoRepository;
            _notificador = notificador;
            _mapper = mapper;
        }

        [HttpGet("Fase/{faseId}")]
        public async Task<ActionResult<IEnumerable<ResultadoFaseDto>>> GetResultadosPorFase(int faseId)
        {
            var resultados = await _resultadoRepository.GetByFaseIdAsync(faseId);
            return Ok(_mapper.Map<IEnumerable<ResultadoFaseDto>>(resultados));
        }

        [HttpPut("BatchUpdate")]
        public async Task<ActionResult<IEnumerable<ResultadoFaseDto>>> BatchUpdate(List<ResultadoUpdateDto> dto)
        {
            var aActualizar = new List<Entidades.Entidades.Resultado>();
            foreach(var item in dto)
            {
                var original = await _resultadoRepository.GetByIdAsync(item.Id);
                if(original != null)
                {
                    if (item.TiempoOficial.HasValue) original.TiempoOficial = item.TiempoOficial;
                    if (item.Posicion.HasValue) original.Posicion = item.Posicion;
                    if (item.Carril.HasValue) original.Carril = item.Carril;
                    
                    if (!string.IsNullOrEmpty(item.Estado))
                        original.Estado = (SportTrack_v1.Entidades.Enums.EstadoResultadoEnum)Enum.Parse(typeof(SportTrack_v1.Entidades.Enums.EstadoResultadoEnum), item.Estado);
                    
                    if (original.Inscripcion?.Participante != null && !string.IsNullOrEmpty(item.ParticipanteNombre))
                    {
                        var nameParts = item.ParticipanteNombre.Trim().Split(' ');
                        if (nameParts.Length > 0)
                        {
                            original.Inscripcion.Participante.Nombre = nameParts[0];
                            original.Inscripcion.Participante.Apellido = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "";
                        }
                    }

                    if (original.Inscripcion?.Participante?.Club != null && !string.IsNullOrEmpty(item.ClubSigla))
                    {
                        original.Inscripcion.Participante.Club.Sigla = item.ClubSigla.Trim();
                    }

                    original.FechaActualizacion = DateTime.UtcNow;
                    original.UsuarioActualizacion = HttpContext.User.Identity?.Name ?? "Sistema";
                    
                    aActualizar.Add(original);
                }
            }
            var guardados = await _resultadoRepository.UpdateManyAsync(aActualizar);

            // Notificar cambios vía SignalR
            if (guardados.Any())
            {
                // Notificamos para cada resultado, asociándolo a su EventoPruebaId
                // Nota: Asumimos que todos pertenecen a la misma prueba para simplificar, 
                // o iteramos si fueran de distintas.
                foreach (var r in guardados)
                {
                    // Necesitamos el EventoPruebaId. Usualmente está en Fase -> Etapa -> EventoPruebaId
                    if (r.Fase?.Etapa != null)
                    {
                        await _notificador.NotificarCambioResultado(r.Fase.Etapa.EventoPruebaId, _mapper.Map<ResultadoFaseDto>(r));
                    }
                }
            }

            return Ok(_mapper.Map<IEnumerable<ResultadoFaseDto>>(guardados));
        }
    }

    public class ResultadoUpdateDto
    {
        public int Id { get; set; }
        public TimeSpan? TiempoOficial { get; set; }
        public int? Posicion { get; set; }
        public string? Estado { get; set; }
        public int? Carril { get; set; }
        public string? ParticipanteNombre { get; set; }
        public string? ClubSigla { get; set; }
    }
}
