using System;
using System.Collections.Generic;

namespace SportTrack_v1.Controladores.SaaS.Dtos
{
    public class ClubSaaSStatusDto
    {
        public int ClubId { get; set; }
        public string ClubNombre { get; set; }
        public int? PlanSaaSId { get; set; }
        public string PlanNombre { get; set; }
        
        public int MaxAtletas { get; set; }
        public int AtletasRegistrados { get; set; }
        public int UsuariosCount { get; set; }
        
        public int MaxTorneos { get; set; }
        public int TorneosActivosCount { get; set; }
        public List<TorneoSaaSDetailDto> TorneosActivos { get; set; } = new List<TorneoSaaSDetailDto>();
        
        public bool PlanAlDia { get; set; }
        public bool Activo { get; set; }
    }

    public class TorneoSaaSDetailDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public DateTime Fecha { get; set; }
        public string Estado { get; set; }
    }
}
