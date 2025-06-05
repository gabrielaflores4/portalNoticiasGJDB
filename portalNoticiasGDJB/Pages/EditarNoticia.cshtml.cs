using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using portalNoticiasGDJB.Data;
using portalNoticiasGDJB.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace portalNoticiasGDJB.Pages
{
    [Authorize(Roles = "Admin")]
    public class EditarNoticiaModel : PageModel
    {
        private readonly AppDb _context;
        private readonly IWebHostEnvironment _env;

        public EditarNoticiaModel(AppDb context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [BindProperty]
        public Noticia Noticia { get; set; }

        public SelectList Categorias { get; set; }

        [TempData]
        public string MensajeExito { get; set; }

        [TempData]
        public string MensajeError { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Noticia = await _context.Noticias.FindAsync(id);
            if (Noticia == null)
            {
                MensajeError = "No se encontró la noticia solicitada.";
                return RedirectToPage("/GestionNoticias");
            }

            Categorias = new SelectList(await _context.Categorias.OrderBy(c => c.Nombre).ToListAsync(), "Id", "Nombre");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Categorias = new SelectList(await _context.Categorias.OrderBy(c => c.Nombre).ToListAsync(), "Id", "Nombre");

            if (!ModelState.IsValid)
            {
                MensajeError = "Por favor corrige los errores del formulario.";
                return Page();
            }

            var noticiaEnDb = await _context.Noticias.FindAsync(Noticia.Id);

            if (noticiaEnDb == null)
            {
                MensajeError = "No se encontró la noticia para actualizar.";
                return Page();
            }

            noticiaEnDb.Titulo = Noticia.Titulo;
            noticiaEnDb.Contenido = Noticia.Contenido;
            noticiaEnDb.FechaPublicacion = Noticia.FechaPublicacion;
            noticiaEnDb.CategoriaId = Noticia.CategoriaId;

            if (Noticia.ArchivoImagen != null && Noticia.ArchivoImagen.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = $"{Path.GetFileNameWithoutExtension(Noticia.ArchivoImagen.FileName)}_{System.Guid.NewGuid()}{Path.GetExtension(Noticia.ArchivoImagen.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Noticia.ArchivoImagen.CopyToAsync(fileStream);
                }

                noticiaEnDb.ImagenRuta = "/uploads/" + uniqueFileName;
            }

            await _context.SaveChangesAsync();

            MensajeExito = "Noticia actualizada correctamente.";
            return RedirectToPage("/GestionNoticias");
        }
    }
}
