using System;

namespace SportTrack_v1.Entidades.Entidades
{
    public class Pago
    {
        public int Id { get; set; }
        public string TipoPago { get; set; } = string.Empty; // "ClubAfiliacion", "AtletaAfiliacion", "InscripcionEvento"
        
        public int? ClubId { get; set; }
        public Club? Club { get; set; }
        
        public int? ParticipanteId { get; set; }
        public Participante? Participante { get; set; }
        
        public int? InscripcionId { get; set; }
        public Inscripcion? Inscripcion { get; set; }
        
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; } = DateTime.UtcNow;
        public string? Referencia { get; set; } // Número de transferencia, recibo, etc.
        public string? RegistradoPor { get; set; } // Nombre de usuario del administrador
        public string? Notas { get; set; }
    }
}
