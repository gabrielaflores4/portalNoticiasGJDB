using Microsoft.AspNetCore.Mvc;
using portalNoticiasGDJB.Data;
using Microsoft.EntityFrameworkCore;

namespace portalNoticiasGDJB.Controllers
{
    [Route("imagen")]
    public class ImagenesController : Controller
    {
        private readonly AppDb _context;

        public ImagenesController(AppDb context)
        {
            _context = context;
        }

        [HttpGet("noticia/{id}")]
        public async Task<IActionResult> GetImagenNoticia(int id)
        {
            var noticia = await _context.Noticias
                .AsNoTracking()
                .Select(n => new { n.Id, n.Imagen })
                .FirstOrDefaultAsync(n => n.Id == id);

            if (noticia?.Imagen == null || noticia.Imagen.Length == 0)
            {
                return File("~/images/default-news.jpg", "image/jpeg");
            }

            Response.Headers.Append("Cache-Control", "public,max-age=86400");

            return File(noticia.Imagen, "image/jpg");
        }
    }
}
