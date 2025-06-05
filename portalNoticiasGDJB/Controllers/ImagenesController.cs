using Microsoft.AspNetCore.Mvc;
using portalNoticiasGDJB.Data;
using Microsoft.EntityFrameworkCore;

namespace portalNoticiasGDJB.Controllers
{
    [Route("imagen")]
    public class ImagenesController : Controller
    {
        private readonly AppDb _context;
        private readonly IWebHostEnvironment _environment;

        public ImagenesController(AppDb context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpGet("noticia/{id}")]
        public async Task<IActionResult> GetImagenNoticia(int id)
        {
            var noticia = await _context.Noticias
                .AsNoTracking()
                .Select(n => new { n.Id, n.ImagenRuta })
                .FirstOrDefaultAsync(n => n.Id == id);

            if (noticia == null || string.IsNullOrEmpty(noticia.ImagenRuta))
            {
                var defaultPath = Path.Combine(_environment.WebRootPath, "images", "default-news.jpg");
                var defaultImageBytes = await System.IO.File.ReadAllBytesAsync(defaultPath);
                Response.Headers.Append("Cache-Control", "public,max-age=86400");
                return File(defaultImageBytes, "image/jpg");
            }

            var rutaFisica = Path.Combine(_environment.WebRootPath, noticia.ImagenRuta.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            if (!System.IO.File.Exists(rutaFisica))
            {
                var defaultPath = Path.Combine(_environment.WebRootPath, "images", "default-news.jpg");
                var defaultImageBytes = await System.IO.File.ReadAllBytesAsync(defaultPath);
                Response.Headers.Append("Cache-Control", "public,max-age=86400");
                return File(defaultImageBytes, "image/jpg");
            }

            var imagenBytes = await System.IO.File.ReadAllBytesAsync(rutaFisica);
            Response.Headers.Append("Cache-Control", "public,max-age=86400");
            return File(imagenBytes, "image/jpg");
        }
    }
}