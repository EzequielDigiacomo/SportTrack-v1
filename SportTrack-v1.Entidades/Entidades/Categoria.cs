using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Entidades.Entidades
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int? EdadMin { get; set; }
        public int? EdadMax { get; set; }

        // Navigation properties
        public ICollection<Prueba> Pruebas { get; set; } = new List<Prueba>();
        public ICollection<Participante> Participantes { get; set; } = new List<Participante>();
    }
}
