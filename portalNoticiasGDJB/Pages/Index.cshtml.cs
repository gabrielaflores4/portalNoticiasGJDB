using Microsoft.AspNetCore.Mvc.RazorPages;
using portalNoticiasGDJB.Data;
using portalNoticiasGDJB.Models;
using Microsoft.EntityFrameworkCore;


namespace portalNoticiasGDJB.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AppDb _context;

        public IndexModel(AppDb context)
        {
            _context = context;
        }

        public List<Noticia> Noticias { get; set; }

        public async Task OnGetAsync()
        {
            Noticias = await _context.Noticias
                .AsNoTracking()
                .OrderByDescending(n => n.FechaPublicacion)
                .ToListAsync();
        }
    }
}
