using System.Collections.Generic;

namespace SportTrack_v1.Controladores.SaaS.Dtos
{
    public class GlobalMetricsDto
    {
        public int TotalFederaciones { get; set; }
        public int TotalClubesAfiliados { get; set; }
        public int TotalAtletasGlobales { get; set; }
        public int TorneosActivosGlobales { get; set; }
        public List<MonthlyGrowthDto> CrecimientoMensual { get; set; } = new();
        public List<FederacionMetricDto> TopFederaciones { get; set; } = new();
    }

    public class MonthlyGrowthDto
    {
        public string Mes { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }

    public class FederacionMetricDto
    {
        public string Nombre { get; set; } = string.Empty;
        public int AtletasCount { get; set; }
        public int ClubesCount { get; set; }
    }
}
