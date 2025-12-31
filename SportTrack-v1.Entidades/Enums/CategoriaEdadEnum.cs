using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Entidades.Enums
{
    public enum CategoriaEdadEnum
    {
        [Display(Name = "Pre-infantil (8-10 años)")]
        Preinfantil = 1,

        [Display(Name = "Infantil (11-12 años)")]
        Infantil = 2,

        [Display(Name = "Menor (13-14 años)")]
        Menor = 3,

        [Display(Name = "Cadete (14-15 años)")]
        Cadete = 4,

        [Display(Name = "Junior (16-17 años)")]
        Junior = 5,

        [Display(Name = "Sub-23 (18-22 años)")]
        Sub23 = 6,

        [Display(Name = "Senior (18-35 años)")]
        Senior = 7,

        [Display(Name = "Master A (40-45 años)")]
        MasterA = 8,

        [Display(Name = "Master B (46-50 años)")]
        MasterB = 9,

        [Display(Name = "Master C (50+ años)")]
        MasterC = 10
    }
}
