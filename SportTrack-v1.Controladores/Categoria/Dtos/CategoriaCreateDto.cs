using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Categoria.Dtos
{
    public class CategoriaCreateDto
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;
        public int? EdadMin { get; set; }
        public int? EdadMax { get; set; }
    }
}
