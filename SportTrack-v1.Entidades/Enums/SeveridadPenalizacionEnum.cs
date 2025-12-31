using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Entidades.Enums
{
    public enum SeveridadPenalizacionEnum
    {
        [Display(Name = "Leve")]
        Leve = 1,

        [Display(Name = "Media")]
        Media = 2,

        [Display(Name = "Grave")]
        Grave = 3
    }
}
