using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace portalNoticiasGDJB.Models
{
    [Table("Comentarios")] // Nombre exacto de la tabla en SQL
    public class Comentario
    {
        [Key]
        [Column("id_comentario")]
        public int Id { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Required]
        [Column("contenido")]
        public string Contenido { get; set; }

        [Column("id_noticia")]
        public int NoticiaId { get; set; }

        [ForeignKey("NoticiaId")]
        public Noticia Noticia { get; set; }

        [Column("user_id")]
        public string UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public IdentityUser Usuario { get; set; }
    }
}

