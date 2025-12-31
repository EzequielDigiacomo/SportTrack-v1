using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Entidades.Enums
{
    public enum EstadoEventoEnum
    {
        [Display(Name = "Programada")]
        Programada = 1,

        [Display(Name = "En Curso")]
        EnCurso = 2,

        [Display(Name = "Finalizado")]
        Finalizado = 3,

        [Display(Name = "Cancelado")]
        Cancelado = 4
    }
}
