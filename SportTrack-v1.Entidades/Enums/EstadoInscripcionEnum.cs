using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Entidades.Enums
{
    public enum EstadoInscripcionEnum
    {
        [Display(Name = "Inscrito")]
        Inscrito = 1,

        [Display(Name = "Confirmado")]
        Confirmado = 2,

        [Display(Name = "Retirado")]
        Retirado = 3,

        [Display(Name = "Ausente")]
        Ausente = 4
    }
}
