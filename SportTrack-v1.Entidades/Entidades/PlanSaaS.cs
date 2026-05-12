using System.Collections.Generic;

namespace SportTrack_v1.Entidades.Entidades
{
    public class PlanSaaS
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        
        // Limites
        public int MaxAtletas { get; set; } // -1 para ilimitado
        public int MaxTorneosActivos { get; set; } // -1 para ilimitado
        
        // Features
        public bool ResultadosTiempoReal { get; set; }
        public bool ExportacionExcel { get; set; }
        public bool SoportePrioritario { get; set; }

        // Navegación
        public ICollection<Club> Clubes { get; set; } = new List<Club>();
    }
}
