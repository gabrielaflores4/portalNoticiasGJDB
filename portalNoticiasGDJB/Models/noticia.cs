using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace portalNoticiasGDJB.Models
{
    [Table("Noticias")]
    public class Noticia
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Titulo { get; set; }

        public byte[] Imagen { get; set; }

        [Required]
        public string Contenido { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaPublicacion { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [Required]
        public string UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public IdentityUser Usuario { get; set; }

        [NotMapped]
        public IFormFile ArchivoImagen { get; set; }

        [NotMapped]
        public string TipoContenidoImagen { get; set; }
    }
}


