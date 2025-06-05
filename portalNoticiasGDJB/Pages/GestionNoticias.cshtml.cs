using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using portalNoticiasGDJB.Models;
using portalNoticiasGDJB.Data;
using Microsoft.EntityFrameworkCore;

namespace portalNoticiasGDJB.Pages
{
    [Authorize(Roles = "Admin")]
    public class GestionNoticiasModel : PageModel
    {
        private readonly AppDb _context;

        public GestionNoticiasModel(AppDb context)
        {
            _context = context;
        }

        public IList<Noticia> Noticias { get; set; }

        public async Task OnGetAsync()
        {
            Noticias = await _context.Noticias.OrderByDescending(n => n.FechaPublicacion).ToListAsync();
        }

        public async Task<IActionResult> OnPostEliminarNoticiaAsync(int noticiaId)
        {
            var noticia = await _context.Noticias.FindAsync(noticiaId);
            if (noticia != null)
            {
                _context.Noticias.Remove(noticia);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}
