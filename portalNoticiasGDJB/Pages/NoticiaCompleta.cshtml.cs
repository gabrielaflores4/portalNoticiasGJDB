using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using portalNoticiasGDJB.Data;
using portalNoticiasGDJB.Models;
using System.Security.Claims;

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
            try
            {
                var noticia = await _context.Noticias
                            .AsNoTracking()
                            .Include(n => n.Usuario)
                            .FirstOrDefaultAsync(n => n.Id == Id);

                if (noticia is null)
                {
                    return NotFound();
                }

                Noticia = noticia;

                Comentarios = await _context.Comentarios
                    .AsNoTracking()  
                    .Include(c => c.Usuario)
                    .Where(c => c.NoticiaId == Id)
                    .OrderByDescending(c => c.FechaCreacion)
                    .ToListAsync();

                return Page();
            }
            catch (Exception)
            {
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(ContenidoComentario))
            {
                ModelState.AddModelError("", "El comentario no puede estar vacío.");
            }

            var noticia = await _context.Noticias
                .Include(n => n.Usuario)
                .FirstOrDefaultAsync(n => n.Id == Id);

            if (noticia == null)
            {
                return NotFound();
            }

            var comentarios = await _context.Comentarios
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
                FechaCreacion = DateTime.UtcNow,
                NoticiaId = Id,
                UsuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new InvalidOperationException("Usuario no autenticado")
            };

            _context.Comentarios.Add(nuevoComentario);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { id = Id });
        }
    }
}
