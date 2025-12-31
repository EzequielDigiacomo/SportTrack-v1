using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Entidades.Enums
{
    public enum SexoEnum
    {
        [Display(Name = "Masculino")]
        Masculino = 1,

        [Display(Name = "Femenino")]
        Femenino = 2,

        [Display(Name = "Mixto")]
        Mixto = 3
    }
}
