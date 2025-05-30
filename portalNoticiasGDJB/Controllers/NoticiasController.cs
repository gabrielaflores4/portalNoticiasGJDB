using portalNoticiasGDJB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using portalNoticiasGDJB.Models;

namespace portalNoticiasGDJB.Controllers
{
    public class NoticiasController : Controller
    {
        private readonly AppDb _context;

        public NoticiasController(AppDb context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Noticias.Include(n => n.Usuario).ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,Titulo,Contenido,FechaPublicacion,UsuarioId")] Noticia noticia,
            IFormFile archivoImagen)
        {
            if (ModelState.IsValid)
            {
                if (archivoImagen != null && archivoImagen.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await archivoImagen.CopyToAsync(memoryStream);
                        noticia.Imagen = memoryStream.ToArray();
                    }
                }

                noticia.FechaRegistro = DateTime.Now;
                noticia.UsuarioId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                _context.Add(noticia);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(noticia);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var noticia = await _context.Noticias.FindAsync(id);
            if (noticia == null)
            {
                return NotFound();
            }
            return View(noticia);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,Titulo,Contenido,FechaPublicacion,FechaRegistro,UsuarioId")] Noticia noticia,
            IFormFile archivoImagen)
        {
            if (id != noticia.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (archivoImagen != null && archivoImagen.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await archivoImagen.CopyToAsync(memoryStream);
                            noticia.Imagen = memoryStream.ToArray();
                        }
                    }
                    else
                    {
                        var noticiaExistente = await _context.Noticias.AsNoTracking()
                            .FirstOrDefaultAsync(n => n.Id == id);
                        noticia.Imagen = noticiaExistente?.Imagen;
                    }

                    _context.Update(noticia);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NoticiaExists(noticia.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(noticia);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var noticia = await _context.Noticias
                .FirstOrDefaultAsync(m => m.Id == id);
            if (noticia == null)
            {
                return NotFound();
            }

            return View(noticia);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var noticia = await _context.Noticias.FindAsync(id);
            _context.Noticias.Remove(noticia);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NoticiaExists(int id)
        {
            return _context.Noticias.Any(e => e.Id == id);
        }
    }
}
