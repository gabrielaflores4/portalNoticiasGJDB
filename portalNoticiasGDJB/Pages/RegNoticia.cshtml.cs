using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using portalNoticiasGDJB.Data;
using portalNoticiasGDJB.Models;
using System.ComponentModel.DataAnnotations;

namespace portalNoticiasGDJB.Pages
{
    public class RegNoticiaModel : PageModel
    {
        private readonly AppDb _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public RegNoticiaModel(AppDb context, UserManager<IdentityUser> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        [BindProperty]
        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(255, ErrorMessage = "El título no puede exceder 255 caracteres")]
        public string Titulo { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "El contenido es obligatorio")]
        public string Contenido { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "La fecha de publicación es obligatoria")]
        public DateTime FechaPublicacion { get; set; } = DateTime.Now;

        [BindProperty]
        [Required(ErrorMessage = "La categoría es obligatoria")]
        public int? CategoriaId { get; set; }

        [BindProperty]
        public IFormFile ArchivoImagen { get; set; }

        public List<SelectListItem> Categorias { get; set; }

        [TempData]
        public string MensajeExito { get; set; }

        [TempData]
        public string MensajeError { get; set; }

        public void OnGet()
        {
            CargarCategorias();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                MensajeError = "Por favor corrige los errores en el formulario";
                CargarCategorias();
                return Page();
            }

            try
            {
                var userId = _userManager.GetUserId(User);
                if (userId == null)
                {
                    MensajeError = "Debes iniciar sesión para publicar noticias";
                    return RedirectToPage("/Account/Login");
                }

                string rutaImagen = null;

                if (ArchivoImagen != null && ArchivoImagen.Length > 0)
                {
                    var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(ArchivoImagen.FileName).ToLowerInvariant();

                    if (!extensionesPermitidas.Contains(extension))
                    {
                        MensajeError = "Solo se permiten archivos de imagen (JPG, JPEG, PNG, GIF)";
                        CargarCategorias();
                        return Page();
                    }

                    var nombreArchivo = Guid.NewGuid().ToString() + extension;
                    var rutaCarpeta = Path.Combine(_environment.WebRootPath, "imagenesNoticias");

                    if (!Directory.Exists(rutaCarpeta))
                        Directory.CreateDirectory(rutaCarpeta);

                    var rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);

                    using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                    {
                        await ArchivoImagen.CopyToAsync(stream);
                    }

                    rutaImagen = "/imagenesNoticias/" + nombreArchivo;
                }

                var noticia = new Noticia
                {
                    Titulo = Titulo,
                    Contenido = Contenido,
                    FechaPublicacion = FechaPublicacion,
                    FechaRegistro = DateTime.Now,
                    ImagenRuta = rutaImagen,
                    UsuarioId = userId,
                    CategoriaId = CategoriaId.Value
                };

                _context.Noticias.Add(noticia);
                await _context.SaveChangesAsync();

                MensajeExito = "Noticia publicada exitosamente!";
                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : "No inner exception";
                MensajeError = $"Error al guardar la noticia: {ex.Message} - Detalle: {innerMessage}";
                CargarCategorias();
                return Page();
            }
        }

        private void CargarCategorias()
        {
            Categorias = _context.Categorias
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Nombre
                }).ToList();
        }
    }
}