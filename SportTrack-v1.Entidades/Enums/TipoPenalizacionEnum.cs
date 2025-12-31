using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Entidades.Enums
{
    public enum TipoPenalizacionEnum
    {
        [Display(Name = "Salida Nula")]
        SalidaNula = 1,

        [Display(Name = "Obstaculizo")]
        Obstaculizo = 2,

        [Display(Name = "Fuera de Pista")]
        FueraPista = 3,

        [Display(Name = "Material Inadecuado")]
        MaterialInadecuado = 4,

        [Display(Name = "Comportamiento Antideportivo")]
        ComportamientoAntideportivo = 5,

        [Display(Name = "No llega a la meta")]
        NollegaAMeta = 6
    }
}
