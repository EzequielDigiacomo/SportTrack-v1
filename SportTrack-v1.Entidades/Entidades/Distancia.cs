using SportTrack_v1.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Entidades.Entidades
{
    public class Distancia
    {
        public int Id { get; set; }
        public DistanciaRegataEnum DistanciaRegata { get; set; } // Usando el enum
        public int GapSugerido { get; set; } = 10; // Valor por defecto en minutos

        // Propiedad calculada para los metros
        public int Metros => (int)DistanciaRegata;

        // Propiedad calculada para descripción
        public string Descripcion => DistanciaRegata.GetDisplayName();

        // Navigation property
        public ICollection<Prueba> Pruebas { get; set; } = new List<Prueba>();
    }
}
