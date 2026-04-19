using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Distancia.Dtos
{
    public class DistanciaRegataDto
    {
        public int Id { get; set; }
        public int Metros { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string Valor { get; set; } = string.Empty;
    }
}
