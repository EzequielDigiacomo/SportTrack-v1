using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Entidades.Enums
{
    public enum TipoBoteEnum
    {
        [Display(Name = "K1")]
        KayakIndividual = 1,

        [Display(Name = "K2")]
        KayakDoble = 2,

        [Display(Name = "K4")]
        KayakCuadruple = 3,

        [Display(Name = "C1")]
        CanoaIndividual = 4,

        [Display(Name = "C2")]
        CanoaDoble = 5,

        [Display(Name = "C4")]
        CanoaCuadruple = 6

    }

}
