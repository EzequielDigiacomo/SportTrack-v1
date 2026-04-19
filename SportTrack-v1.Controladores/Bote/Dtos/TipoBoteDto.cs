using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Bote.Dtos
{
    public class TipoBoteDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Valor { get; set; } = string.Empty;
    }
}
