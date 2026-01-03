using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Bote.Dtos
{
    public class BoteCreateDto
    {
        [Required]
        public string Tipo { get; set; } = string.Empty;
    }
}
