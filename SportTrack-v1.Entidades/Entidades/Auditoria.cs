using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportTrack_v1.Entidades.Entidades
{
    [Table("Auditoria")]
    public class Auditoria
    {
        [Key]
        public int Id { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(100)]
        public string Accion { get; set; } // LOGIN, LOGOUT, CREATE_ATHLETE, DELETE_ATHLETE, etc.

        public string Detalle { get; set; } // Informacion extra en JSON o texto plano.

        public string Usuario { get; set; } // Nombre del usuario que realizo la accion.

        [MaxLength(50)]
        public string IP { get; set; } // Direccion IP desde donde se realizo la accion.

        public string Modulo { get; set; } // Auth, Atletas, Eventos, etc.
    }
}
