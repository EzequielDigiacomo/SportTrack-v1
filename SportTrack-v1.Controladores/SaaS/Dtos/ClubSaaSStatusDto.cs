namespace SportTrack_v1.Controladores.SaaS.Dtos
{
    public class ClubSaaSStatusDto
    {
        public int ClubId { get; set; }
        public string ClubNombre { get; set; } = string.Empty;
        public int? PlanSaaSId { get; set; }
        public string PlanNombre { get; set; } = string.Empty;
        public int MaxAtletas { get; set; }
        public int AtletasRegistrados { get; set; }
        public int MaxTorneos { get; set; }
        public int TorneosActivos { get; set; }
        public bool PlanAlDia { get; set; } // Representa si están dentro de los límites
    }
}
