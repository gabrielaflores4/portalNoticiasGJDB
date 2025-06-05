using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace portalNoticiasGDJB.Models
{
    [Table("Reacciones")]
    public class Reaccion
    {
        [Key]
        [Column("id_reaccion")]
        public int Id { get; set; }

        [Column("UsuarioId")]
        public string UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public IdentityUser Usuario { get; set; }

        [Column("NoticiaId")]
        public int NoticiaId { get; set; }

        [ForeignKey("NoticiaId")]
        public Noticia Noticia { get; set; }

        [Column("TipoReaccion")]
        public bool TipoReaccion { get; set; } // true = Like, false = Dislike

        [Column("FechaReaccion")]
        public DateTime FechaReaccion { get; set; } = DateTime.Now;


    }
}