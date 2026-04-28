using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Entidades.Enums
{
    public enum EstadoResultadoEnum
    {
        [Display(Name = "Pendiente")]
        Pendiente = 1,

        [Display(Name = "Preliminar")]
        Preliminar = 2,

        [Display(Name = "Oficial")]
        Oficial = 3,

        [Display(Name = "DSQ")]
        Descalificado = 4,

        [Display(Name = "Revisado")]
        Revisado = 5,

        [Display(Name = "DNS")]
        DNS = 6,

        [Display(Name = "DNF")]
        DNF = 7
    }
}
