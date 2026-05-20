using SportTrack_v1.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Entidades.Entidades
{
    public class Evento
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public DateTime? FechaFin { get; set; }
        public string? Ubicacion { get; set; }
        public EstadoEventoEnum Estado { get; set; } = EstadoEventoEnum.Programada; // Usando enum
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaFinInscripciones { get; set; }
        
        // Propiedad de pertenencia
        public int? ClubId { get; set; }
        public Club? Club { get; set; }

        public bool InscripcionesHabilitadas { get; set; } = true;

        // Reglas de Competencia
        public bool RestringirSoloCategoriaPropia { get; set; } = false; 
        public bool PermitirSub23EnSenior { get; set; } = false;
        public bool PermitirMasterBajarASenior { get; set; } = false;
        public bool PermitirCompletarK4 { get; set; } = false;
        public bool LimitacionBotesAB { get; set; } = false;
        
        // Configuración de Cronograma Inteligente
        public TimeSpan HoraInicioEvento { get; set; } = new TimeSpan(8, 0, 0); // 08:00 default
        public int CarrilesDisponibles { get; set; } = 9;
        public PerfilTiempoEnum PerfilTiempo { get; set; } = PerfilTiempoEnum.Estandar;
        public TimeSpan HoraInicioReceso { get; set; } = new TimeSpan(13, 0, 0); // 13:00 default
        public TimeSpan HoraFinReceso { get; set; } = new TimeSpan(14, 0, 0); // 14:00 default
        public bool SinReceso { get; set; } = false;
        public int GapEntrePruebas { get; set; } = 10;
        public bool PermitirCombinadas { get; set; } = false;
        public bool UsarGapVariable { get; set; } = false;
        public string TimeZoneId { get; set; } = "America/Argentina/Buenos_Aires"; // Default IANA timezone
        
        // Habilitaciones de Configuración
        public string? CategoriasHabilitadas { get; set; } // IDs separadas por coma
        public string? BotesHabilitados { get; set; } // IDs separadas por coma
        public string? DistanciasHabilitadas { get; set; } // IDs separadas por coma

        // Navigation property
        public ICollection<EventoPrueba> EventoPruebas { get; set; } = new List<EventoPrueba>();
    }
}
