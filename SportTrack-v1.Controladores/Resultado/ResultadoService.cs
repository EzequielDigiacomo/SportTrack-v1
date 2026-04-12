using AutoMapper;
using SportTrack_v1.Controladores.Exceptions;
using SportTrack_v1.Controladores.Resultado.Dtos;
using SportTrack_v1.Entidades.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Resultado
{
    public class ResultadoService : IResultadoService
    {
        private readonly IResultadoRepository _resultadoRepository;
        private readonly INotificadorResultados _notificador;
        private readonly IMapper _mapper;

        public ResultadoService(IResultadoRepository resultadoRepository, INotificadorResultados notificador, IMapper mapper)
        {
            _resultadoRepository = resultadoRepository;
            _notificador = notificador;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ResultadoDto>> GetAllResultadosAsync()
        {
            var resultados = await _resultadoRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ResultadoDto>>(resultados);
        }

        public async Task<ResultadoDto> GetResultadoByIdAsync(int id)
        {
            var resultado = await _resultadoRepository.GetByIdAsync(id);
            if (resultado == null) throw new NotFoundException($"Resultado con ID {id} no encontrado");
            return _mapper.Map<ResultadoDto>(resultado);
        }

        public async Task<ResultadoDto> UpsertResultadoAsync(ResultadoCreateDto resultadoDto)
        {
            var existing = await _resultadoRepository.GetByInscripcionIdAsync(resultadoDto.InscripcionId);
            Entidades.Entidades.Resultado result;

            if (existing != null)
            {
                _mapper.Map(resultadoDto, existing);
                result = await _resultadoRepository.UpdateAsync(existing);
            }
            else
            {
                var nuevo = _mapper.Map<Entidades.Entidades.Resultado>(resultadoDto);
                result = await _resultadoRepository.CreateAsync(nuevo);
            }

            // Recargar para tener datos de navegación para el DTO y la notificación
            var fullResult = await _resultadoRepository.GetByIdAsync(result.Id);
            var dto = _mapper.Map<ResultadoDto>(fullResult);

            // Notificar en tiempo real
            if (fullResult?.Inscripcion != null)
            {
                await _notificador.NotificarCambioResultado(fullResult.Inscripcion.EventoPruebaId, dto);
            }

            return dto;
        }

        public async Task<bool> DeleteResultadoAsync(int id)
        {
            var resultado = await _resultadoRepository.GetByIdAsync(id);
            if (resultado == null) throw new NotFoundException($"Resultado con ID {id} no encontrado");
            
            var success = await _resultadoRepository.DeleteAsync(id);
            
            if (success)
            {
                // Notificar eliminación (opcional, podrías enviar el ID con un estado 'Eliminado')
                await _notificador.NotificarCambioResultado(resultado.Inscripcion.EventoPruebaId, new { id, estado = "Eliminado" });
            }
            
            return success;
        }

        public async Task<IEnumerable<ResultadoDto>> GetResultadosByPruebaAsync(int eventoPruebaId)
        {
            var resultados = await _resultadoRepository.GetByEventoPruebaIdAsync(eventoPruebaId);
            return _mapper.Map<IEnumerable<ResultadoDto>>(resultados);
        }
    }
}
