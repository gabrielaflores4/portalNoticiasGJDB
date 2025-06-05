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
                Noticia = await _context.Noticias
                    .AsNoTracking()
                    .Include(n => n.Usuario)
                    .Include(n => n.Reacciones) // ✅ Necesario para contar likes/dislikes
                    .FirstOrDefaultAsync(n => n.Id == Id);

                if (Noticia == null) return NotFound();

                Comentarios = await _context.Comentarios
                    .AsNoTracking()
                    .Include(c => c.Usuario)
                    .Where(c => c.NoticiaId == Id)
                    .OrderByDescending(c => c.FechaCreacion)
                    .ToListAsync();

                return Page();
            }
            catch
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

            Noticia = await _context.Noticias
                .Include(n => n.Usuario)
                .FirstOrDefaultAsync(n => n.Id == Id);

            Comentarios = await _context.Comentarios
                .Include(c => c.Usuario)
                .Where(c => c.NoticiaId == Id)
                .OrderByDescending(c => c.FechaCreacion)
                .ToListAsync();

            if (!ModelState.IsValid)
                return Page();

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

        public async Task<IActionResult> OnPostReaccionarAsync(int NoticiaId, bool Tipo)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var reaccionExistente = await _context.Reacciones
                .FirstOrDefaultAsync(r => r.UsuarioId == userId && r.NoticiaId == NoticiaId);

            if (reaccionExistente != null)
            {
                reaccionExistente.TipoReaccion = Tipo;
                reaccionExistente.FechaReaccion = DateTime.Now;
                _context.Reacciones.Update(reaccionExistente);
            }
            else
            {
                var nuevaReaccion = new Reaccion
                {
                    UsuarioId = userId,
                    NoticiaId = NoticiaId,
                    TipoReaccion = Tipo,
                    FechaReaccion = DateTime.Now
                };
                _context.Reacciones.Add(nuevaReaccion);
            }

            await _context.SaveChangesAsync();
            return RedirectToPage(new { id = NoticiaId });
        }
    }
}
