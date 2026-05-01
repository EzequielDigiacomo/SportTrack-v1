using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Distancia.Dtos
{
    public class DistanciaDto
    {
        public int Id { get; set; }
        public int DistanciaRegata { get; set; }
        public int Metros { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public int GapSugerido { get; set; }
    }
}
