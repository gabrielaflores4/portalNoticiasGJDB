using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages;
using portalNoticiasGDJB.Models;
using portalNoticiasGDJB.Data;
using Microsoft.AspNetCore.Mvc;

public class NoticiasCategoriaModel : PageModel
{
    private readonly AppDb _context;

    public NoticiasCategoriaModel(AppDb context)
    {
        _context = context;
    }

    public List<Noticia> Noticias { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Categoria { get; set; }

    public void OnGet()
    {
        if (string.IsNullOrEmpty(Categoria))
        {
            Noticias = new List<Noticia>();
        }
        else
        {
            var categoriaMinuscula = Categoria.ToLowerInvariant();

            Noticias = _context.Noticias
                .Include(n => n.Categoria)
                .Where(n => n.Categoria != null && n.Categoria.Nombre.ToLower() == categoriaMinuscula)
                .OrderByDescending(n => n.FechaPublicacion)
                .ToList();
        }
    }
}