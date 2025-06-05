using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using portalNoticiasGDJB.Data; // Cambia al namespace donde esté tu DbContext
using portalNoticiasGDJB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalNoticiasGDJB.Pages
{
    public class NoticiaCompletaModel : PageModel
    {
        private readonly AppDb _context;

        public NoticiaCompletaModel(AppDb context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public Noticia Noticia { get; set; }

        public List<Comentario> Comentarios { get; set; }

        [BindProperty]
        public string ContenidoComentario { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Noticia = await _context.Noticias
                .Include(n => n.Usuario)
                .FirstOrDefaultAsync(n => n.Id == Id);

            if (Noticia == null)
            {
                return NotFound();
            }

            Comentarios = await _context.Comentarios
                .Include(c => c.Usuario)
                .Where(c => c.NoticiaId == Id)
                .OrderByDescending(c => c.FechaCreacion)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(ContenidoComentario))
            {
                ModelState.AddModelError("", "El comentario no puede estar vacío.");
            }

            // Cargar la noticia para volver a mostrar la página
            Noticia = await _context.Noticias
                .Include(n => n.Usuario)
                .FirstOrDefaultAsync(n => n.Id == Id);

            if (Noticia == null)
            {
                return NotFound();
            }

            Comentarios = await _context.Comentarios
                .Include(c => c.Usuario)
                .Where(c => c.NoticiaId == Id)
                .OrderByDescending(c => c.FechaCreacion)
                .ToListAsync();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var nuevoComentario = new Comentario
            {
                Contenido = ContenidoComentario,
                FechaCreacion = DateTime.Now,
                NoticiaId = Id,
                UsuarioId = User.Identity.IsAuthenticated ? User.FindFirst("sub")?.Value ?? User.Identity.Name : null
            };

            _context.Comentarios.Add(nuevoComentario);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { id = Id });
        }
    }
}
