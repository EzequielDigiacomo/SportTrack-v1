using System;

namespace SportTrack_v1.Controladores.Pago.Dtos
{
    public class PagoDto
    {
        public int Id { get; set; }
        public string TipoPago { get; set; } = string.Empty;
        public int? ClubId { get; set; }
        public string? ClubNombre { get; set; }
        public int? ParticipanteId { get; set; }
        public string? ParticipanteNombre { get; set; }
        public int? InscripcionId { get; set; }
        public string? EventoNombre { get; set; }
        public string? PruebaNombre { get; set; }
        
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public string? Referencia { get; set; }
        public string? RegistradoPor { get; set; }
        public string? Notas { get; set; }
    }

    public class RegistrarPagoDto
    {
        public string TipoPago { get; set; } = string.Empty; // "ClubAfiliacion", "AtletaAfiliacion", "InscripcionEvento"
        public int? ClubId { get; set; }
        public int? ParticipanteId { get; set; }
        public int? InscripcionId { get; set; }
        public decimal Monto { get; set; }
        public string? Referencia { get; set; }
        public string? Notas { get; set; }
    }
}
