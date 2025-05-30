using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace portalNoticiasGDJB.Models
{
    [Table("Categoria")]
    public class Categoria
    {
        [Key]
        public int Id_Categoria { get; set; }

        [Required]
        public string Nombre { get; set; }
    }
}

