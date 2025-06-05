using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using portalNoticiasGDJB.Models;
using portalNoticiasGDJB.Data;

namespace portalNoticiasGDJB.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AppDb _context;

        public IndexModel(AppDb context)
        {
            _context = context;
        }

        public IList<Noticia> Noticias { get; set; } = new List<Noticia>();

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        public async Task OnGetAsync()
        {
            var query = _context.Noticias.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchString))
            {
                query = query.Where(n => EF.Functions.Like(n.Titulo, $"%{SearchString}%"));
            }

            Noticias = await query
                .OrderByDescending(n => n.FechaPublicacion)
                .ToListAsync();
        }
    }
}
