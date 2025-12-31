using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SportTrack_v1.Entidades.Enums
{
    public enum DistanciaRegataEnum
    {
        [Display(Name = "200m")]
        Metros200 = 1,      
                            
        [Display(Name = "350m")]
        Metros350 = 2,      
                            
        [Display(Name = "400m")]
        Metros400 = 3,      
                            
        [Display(Name = "450m")]
        Metros450 = 4,      
                            
        [Display(Name = "500m")]
        Metros500 = 5,

        [Display(Name = "1000m")]
        Metros1000 = 6,

        [Display(Name = "1500m")]
        Metros1500 = 7,

        [Display(Name = "2000m")]
        Metros2000 = 8,

        [Display(Name = "3000m")]
        Metros3000 = 9,

        [Display(Name = "5000m")]
        Metros5000 = 10,

        [Display(Name = "10000m")]
        Metros10000 = 11,

        [Display(Name = "12000m")]
        Metros12000 = 12,

        [Display(Name = "15000m")]
        Metros15000 = 13,

        [Display(Name = "18000m")]
        Metros18000 = 14,

        [Display(Name = "22000m")]
        Metros22000 = 15,

        [Display(Name = "30000m")]
        Metros30000 = 16,
    }
}
