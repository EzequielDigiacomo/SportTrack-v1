using SportTrack_v1.Controladores.Bote.Dtos;
using SportTrack_v1.Controladores.Categoria.Dtos;
using SportTrack_v1.Controladores.Distancia.Dtos;
using System;
using System.Collections.Generic;

namespace SportTrack_v1.Controladores.Evento.Dtos
{
    public class EventoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public DateTime? FechaFin { get; set; }
        public string? Ubicacion { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaFinInscripciones { get; set; }
        public int? ClubId { get; set; }
        public string? ClubNombre { get; set; }
        public bool InscripcionesHabilitadas { get; set; }
        public bool InscripcionesAbiertas => InscripcionesHabilitadas && Estado == "Programada" && (!FechaFinInscripciones.HasValue || FechaFinInscripciones.Value > DateTime.UtcNow);
        
        // Reglas de Competencia
        public bool RestringirSoloCategoriaPropia { get; set; }
        public bool PermitirSub23EnSenior { get; set; }
        public bool PermitirMasterBajarASenior { get; set; }
        public bool PermitirCompletarK4 { get; set; }
        public bool LimitacionBotesAB { get; set; }
        public string HoraInicioEvento { get; set; } = "08:00";
        public int CarrilesDisponibles { get; set; }
        public string PerfilTiempo { get; set; } = "Estandar";
        public string HoraInicioReceso { get; set; } = "13:00";
        public string HoraFinReceso { get; set; } = "14:00";
        public bool SinReceso { get; set; }
        public int GapEntrePruebas { get; set; } = 10;
        public bool PermitirCombinadas { get; set; }
        public bool UsarGapVariable { get; set; }
        public string TimeZoneId { get; set; } = "America/Argentina/Buenos_Aires";
        public string? CategoriasHabilitadas { get; set; }
        public string? BotesHabilitados { get; set; }
        public string? DistanciasHabilitadas { get; set; }
    }

    public class EventoCreateDto
    {
        public string Nombre { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public DateTime? FechaFin { get; set; }
        public string? Ubicacion { get; set; }
        public DateTime? FechaFinInscripciones { get; set; }
        public bool RestringirSoloCategoriaPropia { get; set; } = false;
        public bool PermitirSub23EnSenior { get; set; } = false;
        public bool PermitirMasterBajarASenior { get; set; } = false;
        public bool PermitirCompletarK4 { get; set; } = false;
        public bool LimitacionBotesAB { get; set; } = false;
        public int? ClubId { get; set; }
        public bool InscripcionesHabilitadas { get; set; } = true;
        public string HoraInicioEvento { get; set; } = "08:00";
        public int CarrilesDisponibles { get; set; } = 9;
        public string PerfilTiempo { get; set; } = "Estandar";
        public string HoraInicioReceso { get; set; } = "13:00";
        public string HoraFinReceso { get; set; } = "14:00";
        public bool SinReceso { get; set; }
        public int GapEntrePruebas { get; set; } = 10;
        public bool PermitirCombinadas { get; set; } = false;
        public bool UsarGapVariable { get; set; } = false;
        public string TimeZoneId { get; set; } = "America/Argentina/Buenos_Aires";
        public string? CategoriasHabilitadas { get; set; }
        public string? BotesHabilitados { get; set; }
        public string? DistanciasHabilitadas { get; set; }
    }

    public class EventoUpdateDto
    {
        public string? Nombre { get; set; }
        public DateTime? Fecha { get; set; }
        public DateTime? FechaFin { get; set; }
        public string? Ubicacion { get; set; }
        public string? Estado { get; set; }
        public DateTime? FechaFinInscripciones { get; set; }
        public bool? RestringirSoloCategoriaPropia { get; set; }
        public bool? PermitirSub23EnSenior { get; set; }
        public bool? PermitirMasterBajarASenior { get; set; }
        public bool? PermitirCompletarK4 { get; set; }
        public bool? LimitacionBotesAB { get; set; }
        public bool? InscripcionesHabilitadas { get; set; }
        public int? ClubId { get; set; }
        public string? HoraInicioEvento { get; set; }
        public int? CarrilesDisponibles { get; set; }
        public string? PerfilTiempo { get; set; }
        public string? HoraInicioReceso { get; set; }
        public string? HoraFinReceso { get; set; }
        public bool? SinReceso { get; set; }
        public int? GapEntrePruebas { get; set; }
        public bool? PermitirCombinadas { get; set; }
        public bool? UsarGapVariable { get; set; }
        public string? TimeZoneId { get; set; }
        public string? CategoriasHabilitadas { get; set; }
        public string? BotesHabilitados { get; set; }
        public string? DistanciasHabilitadas { get; set; }
    }

    public class EventoPruebaDto
    {
        public int Id { get; set; }
        public int EventoId { get; set; }
        public int PruebaId { get; set; }
        public PruebaDto? Prueba { get; set; }
        public DateTime FechaHora { get; set; }
        public string? Estado { get; set; }
        public int CantidadInscritos { get; set; }
        public string? PlanProgresionAsignado { get; set; }
    }

    public class EventoPruebaCreateDto
    {
        public int CategoriaId { get; set; }
        public int BoteId { get; set; }
        public int DistanciaId { get; set; }
        public int SexoId { get; set; } = 1; // Mixto por defecto
        public DateTime? FechaHora { get; set; }
    }

    public class PruebaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public CategoriaDto Categoria { get; set; } = null!;
        public BoteDto Bote { get; set; } = null!;
        public DistanciaDto Distancia { get; set; } = null!;
        public SexoDto? Sexo { get; set; }
        public string SexoNombre { get; set; } = string.Empty;
        public int SexoId { get; set; }
    }

    public class SexoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}
