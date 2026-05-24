using Microsoft.EntityFrameworkCore;
using SportTrack.AccessDatos;
using SportTrack_v1.Controladores.Audit;
using SportTrack_v1.Controladores.Exceptions;
using SportTrack_v1.Controladores.Pago.Dtos;
using SportTrack_v1.Entidades.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Pago
{
    public class PagoService : IPagoService
    {
        private readonly SportTrackDbContext _context;
        private readonly IAuditService _auditService;

        public PagoService(SportTrackDbContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<IEnumerable<PagoDto>> GetHistorialPagosAsync(int? fedId, string? role)
        {
            var query = _context.Pagos
                .Include(p => p.Club)
                .Include(p => p.Participante)
                    .ThenInclude(pa => pa.Club)
                .Include(p => p.Inscripcion)
                    .ThenInclude(i => i.EventoPrueba)
                        .ThenInclude(ep => ep.Evento)
                .Include(p => p.Inscripcion)
                    .ThenInclude(i => i.EventoPrueba)
                        .ThenInclude(ep => ep.Prueba)
                            .ThenInclude(pr => pr.Categoria)
                .Include(p => p.Inscripcion)
                    .ThenInclude(i => i.EventoPrueba)
                        .ThenInclude(ep => ep.Prueba)
                            .ThenInclude(pr => pr.Bote)
                .Include(p => p.Inscripcion)
                    .ThenInclude(i => i.EventoPrueba)
                        .ThenInclude(ep => ep.Prueba)
                            .ThenInclude(pr => pr.Distancia)
                .AsQueryable();

            if (role == "Admin" && fedId.HasValue)
            {
                // Un admin federativo solo ve pagos de:
                // 1. Su propio club/federación o clubes hijos (ParentClubId == fedId)
                // 3. Atletas cuyos clubes son hijos de la federación o su propio club
                // 4. Inscripciones en eventos de su federación (Evento.ClubId == fedId)
                query = query.Where(p =>
                    (p.ClubId == fedId.Value || (p.Club != null && p.Club.ParentClubId == fedId.Value)) ||
                    (p.Participante != null && (p.Participante.ClubId == fedId.Value || (p.Participante.Club != null && p.Participante.Club.ParentClubId == fedId.Value))) ||
                    (p.Inscripcion != null && p.Inscripcion.EventoPrueba != null && p.Inscripcion.EventoPrueba.Evento != null && p.Inscripcion.EventoPrueba.Evento.ClubId == fedId.Value)
                );
            }
            else if (role != "SuperAdmin" && role != "soporte_tecnico")
            {
                // Otros roles menores (como Club Admin) solo ven los pagos de su propio Club y Atletas
                if (fedId.HasValue)
                {
                    query = query.Where(p =>
                        p.ClubId == fedId.Value ||
                        (p.Participante != null && p.Participante.ClubId == fedId.Value) ||
                        (p.Inscripcion != null && p.Inscripcion.Participante != null && p.Inscripcion.Participante.ClubId == fedId.Value)
                    );
                }
                else
                {
                    return Enumerable.Empty<PagoDto>();
                }
            }

            var pagos = await query
                .OrderByDescending(p => p.FechaPago)
                .ToListAsync();

            return pagos.Select(p => new PagoDto
            {
                Id = p.Id,
                TipoPago = p.TipoPago,
                ClubId = p.ClubId,
                ClubNombre = p.Club?.Nombre,
                ParticipanteId = p.ParticipanteId,
                ParticipanteNombre = p.Participante != null ? $"{p.Participante.Nombre} {p.Participante.Apellido}" : null,
                InscripcionId = p.InscripcionId,
                EventoNombre = p.Inscripcion?.EventoPrueba?.Evento?.Nombre,
                PruebaNombre = p.Inscripcion?.EventoPrueba?.Prueba != null 
                    ? $"{p.Inscripcion.EventoPrueba.Prueba.Categoria?.Nombre} {p.Inscripcion.EventoPrueba.Prueba.Bote?.Tipo} {p.Inscripcion.EventoPrueba.Prueba.Distancia?.Descripcion}".Trim()
                    : null,
                Monto = p.Monto,
                FechaPago = p.FechaPago,
                Referencia = p.Referencia,
                RegistradoPor = p.RegistradoPor,
                Notas = p.Notas
            });
        }

        public async Task<PagoDto> RegistrarPagoAsync(RegistrarPagoDto dto, string registradoPor)
        {
            var pago = new Entidades.Entidades.Pago
            {
                TipoPago = dto.TipoPago,
                Monto = dto.Monto,
                Referencia = dto.Referencia,
                Notas = dto.Notas,
                RegistradoPor = registradoPor,
                FechaPago = DateTime.UtcNow
            };

            string detalleAuditoria = "";

            if (dto.TipoPago == "ClubAfiliacion")
            {
                if (!dto.ClubId.HasValue) throw new BadRequestException("ClubId es requerido para afiliación de club");
                var club = await _context.Clubes.FindAsync(dto.ClubId.Value);
                if (club == null) throw new NotFoundException($"Club con ID {dto.ClubId.Value} no encontrado");
                
                club.PagoAfiliacionAlDia = true;
                club.SolicitudPagoPendiente = false;
                pago.ClubId = club.Id;
                detalleAuditoria = $"Pago de afiliación anual de Club '{club.Nombre}' registrado por ${dto.Monto} (Ref: {dto.Referencia}).";
            }
            else if (dto.TipoPago == "AtletaAfiliacion")
            {
                if (!dto.ParticipanteId.HasValue) throw new BadRequestException("ParticipanteId es requerido para afiliación de atleta");
                var atleta = await _context.Participantes.FindAsync(dto.ParticipanteId.Value);
                if (atleta == null) throw new NotFoundException($"Atleta con ID {dto.ParticipanteId.Value} no encontrado");

                atleta.PagoAfiliacionAlDia = true;
                pago.ParticipanteId = atleta.Id;
                detalleAuditoria = $"Pago de afiliación de Atleta '{atleta.Nombre} {atleta.Apellido}' registrado por ${dto.Monto} (Ref: {dto.Referencia}).";
            }
            else if (dto.TipoPago == "InscripcionEvento")
            {
                if (!dto.InscripcionId.HasValue) throw new BadRequestException("InscripcionId es requerido para pago de inscripción");
                var inscripcion = await _context.Inscripciones
                    .Include(i => i.Participante)
                    .Include(i => i.EventoPrueba)
                        .ThenInclude(ep => ep.Evento)
                    .FirstOrDefaultAsync(i => i.Id == dto.InscripcionId.Value);
                if (inscripcion == null) throw new NotFoundException($"Inscripción con ID {dto.InscripcionId.Value} no encontrada");

                inscripcion.Pagado = true;
                pago.InscripcionId = inscripcion.Id;
                string atletaName = inscripcion.Participante != null ? $"{inscripcion.Participante.Nombre} {inscripcion.Participante.Apellido}" : "Atleta";
                string eventoName = inscripcion.EventoPrueba?.Evento?.Nombre ?? "Evento";
                detalleAuditoria = $"Pago de inscripción de {atletaName} a regata en '{eventoName}' registrado por ${dto.Monto} (Ref: {dto.Referencia}).";
            }
            else
            {
                throw new BadRequestException($"Tipo de pago '{dto.TipoPago}' inválido");
            }

            _context.Pagos.Add(pago);
            await _context.SaveChangesAsync();

            await _auditService.RegistrarAccionAsync("REGISTRAR_PAGO", detalleAuditoria, registradoPor, "Pagos");

            // Cargar datos para retornar DTO completo
            var pagoCargado = await _context.Pagos
                .Include(p => p.Club)
                .Include(p => p.Participante)
                .Include(p => p.Inscripcion)
                    .ThenInclude(i => i.EventoPrueba)
                        .ThenInclude(ep => ep.Evento)
                .FirstOrDefaultAsync(p => p.Id == pago.Id);

            return new PagoDto
            {
                Id = pago.Id,
                TipoPago = pago.TipoPago,
                ClubId = pago.ClubId,
                ClubNombre = pagoCargado?.Club?.Nombre,
                ParticipanteId = pago.ParticipanteId,
                ParticipanteNombre = pagoCargado?.Participante != null ? $"{pagoCargado.Participante.Nombre} {pagoCargado.Participante.Apellido}" : null,
                InscripcionId = pago.InscripcionId,
                EventoNombre = pagoCargado?.Inscripcion?.EventoPrueba?.Evento?.Nombre,
                Monto = pago.Monto,
                FechaPago = pago.FechaPago,
                Referencia = pago.Referencia,
                RegistradoPor = pago.RegistradoPor,
                Notas = pago.Notas
            };
        }

        public async Task<bool> ToggleClubPagoStatusAsync(int clubId, bool alDia)
        {
            var club = await _context.Clubes.FindAsync(clubId);
            if (club == null) throw new NotFoundException($"Club con ID {clubId} no encontrado");

            club.PagoAfiliacionAlDia = alDia;
            if (alDia)
            {
                club.SolicitudPagoPendiente = false;
            }
            _context.Clubes.Update(club);
            var result = await _context.SaveChangesAsync() > 0;

            if (result)
            {
                string estadoStr = alDia ? "Al Día" : "Deudor";
                await _auditService.RegistrarAccionAsync("TOGGLE_PAGO_CLUB", $"Se cambió el estado de afiliación del Club '{club.Nombre}' a '{estadoStr}'", null, "Pagos");
            }

            return result;
        }

        public async Task<bool> SetSolicitudPagoPendienteAsync(int clubId, bool pendiente)
        {
            var club = await _context.Clubes.FindAsync(clubId);
            if (club == null) throw new NotFoundException($"Club con ID {clubId} no encontrado");

            club.SolicitudPagoPendiente = pendiente;
            _context.Clubes.Update(club);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ToggleAtletaPagoStatusAsync(int participanteId, bool alDia)
        {
            var atleta = await _context.Participantes.FindAsync(participanteId);
            if (atleta == null) throw new NotFoundException($"Atleta con ID {participanteId} no encontrado");

            atleta.PagoAfiliacionAlDia = alDia;
            _context.Participantes.Update(atleta);
            var result = await _context.SaveChangesAsync() > 0;

            if (result)
            {
                string estadoStr = alDia ? "Al Día" : "Deudor";
                await _auditService.RegistrarAccionAsync("TOGGLE_PAGO_ATLETA", $"Se cambió el estado de afiliación del Atleta '{atleta.Nombre} {atleta.Apellido}' a '{estadoStr}'", null, "Pagos");
            }

            return result;
        }

        public async Task<bool> ToggleInscripcionPagoStatusAsync(int inscripcionId, bool pagado)
        {
            var inscripcion = await _context.Inscripciones
                .Include(i => i.Participante)
                .Include(i => i.EventoPrueba)
                    .ThenInclude(ep => ep.Evento)
                .FirstOrDefaultAsync(i => i.Id == inscripcionId);
            if (inscripcion == null) throw new NotFoundException($"Inscripción con ID {inscripcionId} no encontrada");

            inscripcion.Pagado = pagado;
            _context.Inscripciones.Update(inscripcion);
            var result = await _context.SaveChangesAsync() > 0;

            if (result)
            {
                string estadoStr = pagado ? "Pagado" : "Pendiente";
                string atletaName = inscripcion.Participante != null ? $"{inscripcion.Participante.Nombre} {inscripcion.Participante.Apellido}" : "Atleta";
                string eventoName = inscripcion.EventoPrueba?.Evento?.Nombre ?? "Evento";
                await _auditService.RegistrarAccionAsync("TOGGLE_PAGO_INSCRIPCION", $"Se cambió el estado de pago de inscripción de '{atletaName}' para '{eventoName}' a '{estadoStr}'", null, "Pagos");
            }

            return result;
        }
    }
}
