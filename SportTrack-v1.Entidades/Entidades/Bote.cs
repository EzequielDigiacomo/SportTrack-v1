using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Entidades.Entidades
{
    public class Bote
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = string.Empty;

        // Navigation property
        public ICollection<Prueba> Pruebas { get; set; } = new List<Prueba>();
    }
}
