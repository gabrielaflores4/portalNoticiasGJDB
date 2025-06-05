using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace portalNoticiasGDJB.Models
{
    [Table("Noticia")]
    public class Noticia
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Titulo { get; set; }

        [StringLength(500)]
        public string ImagenRuta { get; set; }  

        [Required]
        public string Contenido { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime FechaPublicacion { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [Required]
        public string UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public IdentityUser Usuario { get; set; }

        [Column("id_categoria")]
        public int? CategoriaId { get; set; }

        [ForeignKey("CategoriaId")]
        public Categoria Categoria { get; set; }

        [NotMapped]
        public IFormFile ArchivoImagen { get; set; }

        public ICollection<Comentario> Comentarios { get; set; }
        public ICollection<Guardado> Guardados { get; set; }
        public ICollection<Reaccion> Reacciones { get; set; }
    }
}


