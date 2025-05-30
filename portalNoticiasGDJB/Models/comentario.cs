using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace portalNoticiasGDJB.Models
{
    [Table("Comentarios")]
    public class Comentario
    {
        public int Id { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string Contenido { get; set; }

        public int NoticiaId { get; set; }
        public Noticia Noticia { get; set; }

        public string UsuarioId { get; set; }
        public IdentityUser Usuario { get; set; }
    }
}

