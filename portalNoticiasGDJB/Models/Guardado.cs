using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace portalNoticiasGDJB.Models
{
    [Table("Guardados")]
    public class Guardado
    {
        [Key]
        [Column("id_guardado")]
        public int Id { get; set; }

        [Column("UsuarioId")]
        public string UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public IdentityUser Usuario { get; set; }

        [Column("NoticiaId")]
        public int NoticiaId { get; set; }

        [ForeignKey("NoticiaId")]
        public Noticia Noticia { get; set; }

        [Column("FechaGuardado")]
        public DateTime FechaGuardado { get; set; } = DateTime.Now;
    }
}