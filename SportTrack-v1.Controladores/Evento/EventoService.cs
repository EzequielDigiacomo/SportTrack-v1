using AutoMapper;
using SportTrack_v1.Controladores.Evento.Dtos;
using SportTrack_v1.Controladores.Exceptions;
using SportTrack_v1.Entidades.Entidades;
using SportTrack_v1.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Evento
{
    public class EventoService : IEventoService
    {
        private readonly IEventoRepository _eventoRepository;
        private readonly IMapper _mapper;

        public EventoService(IEventoRepository eventoRepository, IMapper mapper)
        {
            _eventoRepository = eventoRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EventoDto>> GetAllEventosAsync()
        {
            var eventos = await _eventoRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<EventoDto>>(eventos);
        }

        public async Task<EventoDto> GetEventoByIdAsync(int id)
        {
            var evento = await _eventoRepository.GetByIdAsync(id);
            if (evento == null) throw new NotFoundException($"Evento con ID {id} no encontrado");
            return _mapper.Map<EventoDto>(evento);
        }

        public async Task<EventoDto> CreateEventoAsync(EventoCreateDto eventoDto)
        {
            var evento = _mapper.Map<Entidades.Entidades.Evento>(eventoDto);
            
            // Forzar fecha a UTC para evitar error de Npgsql (timestamp with time zone)
            evento.Fecha = DateTime.SpecifyKind(evento.Fecha, DateTimeKind.Utc);
            
            var result = await _eventoRepository.CreateAsync(evento);
            return _mapper.Map<EventoDto>(result);
        }

        public async Task<EventoDto> UpdateEventoAsync(int id, EventoUpdateDto eventoDto)
        {
            var existing = await _eventoRepository.GetByIdAsync(id);
            if (existing == null) throw new NotFoundException($"Evento con ID {id} no encontrado");
            
            _mapper.Map(eventoDto, existing);
            var result = await _eventoRepository.UpdateAsync(existing);
            return _mapper.Map<EventoDto>(result);
        }

        public async Task<bool> DeleteEventoAsync(int id)
        {
            if (!await _eventoRepository.ExistsAsync(id)) throw new NotFoundException($"Evento con ID {id} no encontrado");
            return await _eventoRepository.DeleteAsync(id);
        }
        public async Task<IEnumerable<EventoDto>> GetProximosEventosAsync()
        {
            var eventos = await _eventoRepository.GetProximosAsync();
            return _mapper.Map<IEnumerable<EventoDto>>(eventos);
        }

        public async Task<IEnumerable<EventoPruebaDto>> GetPruebasByEventoAsync(int eventoId)
        {
            var pruebas = await _eventoRepository.GetPruebasByEventoIdAsync(eventoId);
            return _mapper.Map<IEnumerable<EventoPruebaDto>>(pruebas);
        }

        public async Task<EventoPruebaDto> AssignPruebaToEventoAsync(int eventoId, EventoPruebaCreateDto assignDto)
        {
            // 1. Buscar si la prueba técnica ya existe por sus IDs
            var prueba = await _eventoRepository.GetPruebaAsync(assignDto.CategoriaId, assignDto.BoteId, assignDto.DistanciaId, assignDto.SexoId);

            if (prueba == null)
            {
                // 2. Si no existe, crearla. Consultamos maestros para el nombre.
                // Por ahora usamos IDs directamente en el nombre o consultamos repositorios si fuera necesario.
                // Simplificado: Creamos la prueba con los IDs enviados.
                prueba = new Prueba
                {
                    CategoriaId = assignDto.CategoriaId,
                    BoteId = assignDto.BoteId,
                    DistanciaId = assignDto.DistanciaId,
                    SexoId = assignDto.SexoId,
                    Nombre = $"Prueba {assignDto.CategoriaId}-{assignDto.BoteId}-{assignDto.DistanciaId}"
                };
                prueba = await _eventoRepository.CreatePruebaAsync(prueba);
            }

            // 3. Vincular al evento
            var eventoPrueba = new EventoPrueba
            {
                EventoId = eventoId,
                PruebaId = prueba.Id,
                FechaHora = assignDto.FechaHora ?? DateTime.UtcNow,
                Estado = EstadoEventoEnum.Programada
            };

            // Asegurar UTC para la fecha de la prueba
            eventoPrueba.FechaHora = DateTime.SpecifyKind(eventoPrueba.FechaHora, DateTimeKind.Utc);

            var result = await _eventoRepository.AssignPruebaAsync(eventoPrueba);
            
            // Recargamos para traer las navegaciones (Categoria, Bote, Distancia) si el Repo lo permite
            // o mapeamos lo que tenemos.
            // Recargamos para traer las navegaciones (Categoria, Bote, Distancia) si el Repo lo permite
            // o mapeamos lo que tenemos.
            return _mapper.Map<EventoPruebaDto>(result);
        }

        public async Task<EventoPruebaDto> UpdateEventoPruebaAsync(int eventoPruebaId, EventoPruebaCreateDto updateDto)
        {
            var existing = await _eventoRepository.GetEventoPruebaByIdAsync(eventoPruebaId);
            if (existing == null) throw new NotFoundException($"Asignación {eventoPruebaId} no encontrada");

            // 1. Buscar/Crear la prueba técnica si cambiaron los parámetros
            var prueba = await _eventoRepository.GetPruebaAsync(updateDto.CategoriaId, updateDto.BoteId, updateDto.DistanciaId, updateDto.SexoId);
            if (prueba == null)
            {
                prueba = new Prueba
                {
                    CategoriaId = updateDto.CategoriaId,
                    BoteId = updateDto.BoteId,
                    DistanciaId = updateDto.DistanciaId,
                    SexoId = updateDto.SexoId,
                    Nombre = $"Prueba {updateDto.CategoriaId}-{updateDto.BoteId}-{updateDto.DistanciaId}"
                };
                prueba = await _eventoRepository.CreatePruebaAsync(prueba);
            }

            // 2. Actualizar la asignación
            existing.PruebaId = prueba.Id;
            existing.FechaHora = updateDto.FechaHora ?? existing.FechaHora;
            existing.FechaHora = DateTime.SpecifyKind(existing.FechaHora, DateTimeKind.Utc);

            var result = await _eventoRepository.UpdateEventoPruebaAsync(existing);
            return _mapper.Map<EventoPruebaDto>(result);
        }

        public async Task<bool> DeleteEventoPruebaAsync(int eventoPruebaId)
        {
            return await _eventoRepository.UnassignPruebaAsync(eventoPruebaId);
        }
    }
}
