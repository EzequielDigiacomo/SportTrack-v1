using AutoMapper;
using SportTrack_v1.Controladores.Club.Dtos;
using SportTrack_v1.Controladores.Exceptions;
using SportTrack_v1.Controladores.Audit;
using SportTrack_v1.Entidades.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Club
{
    public class ClubService : IClubService
    {
        private readonly IClubRepository _clubRepository;
        private readonly IMapper _mapper;
        private readonly IAuditService _auditService;

        public ClubService(IClubRepository clubRepository, IMapper mapper, IAuditService auditService)
        {
            _clubRepository = clubRepository;
            _mapper = mapper;
            _auditService = auditService;
        }

        public async Task<IEnumerable<ClubDto>> GetAllClubesAsync()
        {
            var clubes = await _clubRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ClubDto>>(clubes);
        }

        public async Task<ClubDto> GetClubByIdAsync(int id)
        {
            var club = await _clubRepository.GetByIdAsync(id);
            if (club == null) throw new NotFoundException($"Club con ID {id} no encontrado");
            return _mapper.Map<ClubDto>(club);
        }

        public async Task<ClubDto> CreateClubAsync(ClubCreateDto clubDto)
        {
            var club = _mapper.Map<Entidades.Entidades.Club>(clubDto);
            
            // Asignar plan por defecto (Bronce = 1) si no tiene uno
            if (club.PlanSaaSId == null || club.PlanSaaSId == 0)
            {
                club.PlanSaaSId = 1; 
            }

            var result = await _clubRepository.CreateAsync(club);
            return _mapper.Map<ClubDto>(result);
        }

        public async Task<ClubDto> UpdateClubAsync(int id, ClubUpdateDto clubDto)
        {
            var existing = await _clubRepository.GetByIdAsync(id);
            if (existing == null) throw new NotFoundException($"Club con ID {id} no encontrado");
            
            // Capturar valores anteriores para auditoría
            var oldVencimiento = existing.FechaVencimientoPlan;
            var oldBloqueado = existing.BloqueadoPorFaltaDePago;
            var oldActivo = existing.Activo;

            _mapper.Map(clubDto, existing);
            var result = await _clubRepository.UpdateAsync(existing);

            // Registrar auditoría si es una federación raíz (ParentClubId == null)
            if (existing.ParentClubId == null)
            {
                if (existing.FechaVencimientoPlan != oldVencimiento && existing.FechaVencimientoPlan > oldVencimiento)
                {
                    string freq = existing.FrecuenciaPago ?? "Mensual";
                    string vencimientoStr = existing.FechaVencimientoPlan.HasValue ? existing.FechaVencimientoPlan.Value.ToShortDateString() : "Sin fecha";
                    await _auditService.RegistrarAccionAsync(
                        "RENEW_PLAN", 
                        $"Plan de federación '{existing.Nombre}' renovado ({freq}). Nuevo vencimiento: {vencimientoStr}.", 
                        modulo: "SaaS"
                    );
                }
                
                if (existing.BloqueadoPorFaltaDePago != oldBloqueado)
                {
                    string accion = existing.BloqueadoPorFaltaDePago ? "BLOCK_FEDERATION" : "UNBLOCK_FEDERATION";
                    string detalle = existing.BloqueadoPorFaltaDePago 
                        ? $"Federación '{existing.Nombre}' bloqueada por falta de pago." 
                        : $"Federación '{existing.Nombre}' desbloqueada por pago al día.";
                    await _auditService.RegistrarAccionAsync(accion, detalle, modulo: "SaaS");
                }
                
                if (existing.Activo != oldActivo)
                {
                    string status = existing.Activo ? "habilitado" : "suspendido";
                    string accion = existing.Activo ? "ACTIVATE_FEDERATION" : "SUSPEND_FEDERATION";
                    await _auditService.RegistrarAccionAsync(
                        accion, 
                        $"Acceso a la federación '{existing.Nombre}' {status} manualmente.", 
                        modulo: "SaaS"
                    );
                }
            }

            return _mapper.Map<ClubDto>(result);
        }

        public async Task<bool> DeleteClubAsync(int id)
        {
            if (!await _clubRepository.ExistsAsync(id)) throw new NotFoundException($"Club con ID {id} no encontrado");
            return await _clubRepository.DeleteAsync(id);
        }
    }
}
