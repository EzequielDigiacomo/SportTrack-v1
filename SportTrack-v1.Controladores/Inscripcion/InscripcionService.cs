using AutoMapper;
using SportTrack_v1.Controladores.Inscripcion.Dtos;
using SportTrack_v1.Controladores.Inscripcion;
using SportTrack_v1.Entidades.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SportTrack_v1.Controladores.Exceptions;

namespace SportTrack_v1.Controladores.Inscripcion
{
    public class InscripcionService : IInscripcionService
    {
        private readonly IInscripcionRepository _inscripcionRepository;
        private readonly IMapper _mapper;
        private readonly Audit.IAuditService _auditService;

        public InscripcionService(IInscripcionRepository inscripcionRepository, IMapper mapper, Audit.IAuditService auditService)
        {
            _inscripcionRepository = inscripcionRepository;
            _mapper = mapper;
            _auditService = auditService;
        }

        public async Task<IEnumerable<InscripcionDto>> GetAllInscripcionesAsync()
        {
            var inscripciones = await _inscripcionRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<InscripcionDto>>(inscripciones);
        }

        public async Task<InscripcionDto> GetInscripcionByIdAsync(int id)
        {
            var inscripcion = await _inscripcionRepository.GetByIdAsync(id);
            if (inscripcion == null)
                throw new NotFoundException($"Inscripción con ID {id} no encontrada");

            return _mapper.Map<InscripcionDto>(inscripcion);
        }

        public async Task<InscripcionDto> CreateInscripcionAsync(InscripcionCreateDto inscripcionDto)
        {
            var inscripcion = _mapper.Map<Entidades.Entidades.Inscripcion>(inscripcionDto);
            var createdInscripcion = await _inscripcionRepository.CreateAsync(inscripcion);
            
            // Recargamos para incluir los datos relacionados (Participante) en el DTO de salida
            var result = await _inscripcionRepository.GetByIdAsync(createdInscripcion.Id);
            
            // Auditoria
            await _auditService.RegistrarAccionAsync("CREATE_INSCRIPTION", 
                $"Nueva inscripción: {result.Participante?.Nombre} {result.Participante?.Apellido} (ID: {result.Id})", null, "Inscripciones");

            return _mapper.Map<InscripcionDto>(result);
        }

        public async Task<InscripcionDto> UpdateInscripcionAsync(int id, InscripcionUpdateDto inscripcionDto)
        {
            var existingInscripcion = await _inscripcionRepository.GetByIdAsync(id);
            if (existingInscripcion == null)
                throw new NotFoundException($"Inscripción con ID {id} no encontrada");

            // Patch manual: solo sobrescribir los campos que vienen con valor
            if (inscripcionDto.EventoPruebaId.HasValue)
                existingInscripcion.EventoPruebaId = inscripcionDto.EventoPruebaId.Value;

            if (inscripcionDto.NumeroCompetidor != null)
                existingInscripcion.NumeroCompetidor = inscripcionDto.NumeroCompetidor;

            var updatedInscripcion = await _inscripcionRepository.UpdateAsync(existingInscripcion);
            
            // Auditoria
            await _auditService.RegistrarAccionAsync("UPDATE_INSCRIPTION", 
                $"Inscripción actualizada (ID: {id}, Carril: {updatedInscripcion.NumeroCompetidor})", null, "Inscripciones");

            return _mapper.Map<InscripcionDto>(updatedInscripcion);
        }

        public async Task<bool> DeleteInscripcionAsync(int id)
        {
            if (!await _inscripcionRepository.ExistsAsync(id))
                throw new NotFoundException($"Inscripción con ID {id} no encontrada");

            var res = await _inscripcionRepository.DeleteAsync(id);
            
            // Auditoria
            if (res) {
                await _auditService.RegistrarAccionAsync("DELETE_INSCRIPTION", 
                    $"Inscripción eliminada (ID: {id})", null, "Inscripciones");
            }

            return res;
        }

        public async Task<int> GetCountByEventoPruebaIdAsync(int eventoPruebaId)
        {
            return await _inscripcionRepository.CountByEventoPruebaIdAsync(eventoPruebaId);
        }

        public async Task<IEnumerable<InscripcionDto>> GetInscripcionesByEventoPruebaIdAsync(int eventoPruebaId)
        {
            var inscripciones = await _inscripcionRepository.GetByEventoPruebaIdAsync(eventoPruebaId);
            return _mapper.Map<IEnumerable<InscripcionDto>>(inscripciones);
        }
        public async Task<IEnumerable<InscripcionDto>> GetInscripcionesByEventoAndClubAsync(int eventoId, int clubId)
        {
            var inscripciones = await _inscripcionRepository.GetByEventoAndClubAsync(eventoId, clubId);
            return _mapper.Map<IEnumerable<InscripcionDto>>(inscripciones);
        }

        public async Task<bool> ToggleEsCabezaDeSerieAsync(int id)
        {
            var inscripcion = await _inscripcionRepository.GetByIdAsync(id);
            if (inscripcion == null) throw new NotFoundException($"Inscripción {id} no encontrada");

            // Si se intenta activar (pasar de false a true)
            if (!inscripcion.EsCabezaDeSerie)
            {
                var inscripcionesEnPrueba = await _inscripcionRepository.GetByEventoPruebaIdAsync(inscripcion.EventoPruebaId);
                var totalInscritos = inscripcionesEnPrueba.Count();
                var actualSeeds = inscripcionesEnPrueba.Count(i => i.EsCabezaDeSerie);

                var maxSeedsAllowed = (int)Math.Ceiling(totalInscritos / 9.0);

                if (actualSeeds >= maxSeedsAllowed)
                {
                    throw new BadRequestException($"Límite de cabezas de serie alcanzado. Máximo permitido: {maxSeedsAllowed} para {totalInscritos} atletas.");
                }
            }

            inscripcion.EsCabezaDeSerie = !inscripcion.EsCabezaDeSerie;
            await _inscripcionRepository.UpdateAsync(inscripcion);
            
            // Auditoria
            await _auditService.RegistrarAccionAsync("TOGGLE_SEED", 
                $"Cabeza de serie {(inscripcion.EsCabezaDeSerie ? "activado" : "desactivado")} para Inscripción {id}", null, "Inscripciones");

            return true;
        }
    }
}
