using portalNoticiasGDJB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using portalNoticiasGDJB.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace portalNoticiasGDJB.Controllers
{
    [Authorize]
    public class NoticiasController : Controller
    {
        private readonly AppDb _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<NoticiasController> _logger;
        private const string CarpetaImagenes = "imagenesNoticias";

        public NoticiasController(AppDb context, IWebHostEnvironment env, ILogger<NoticiasController> logger)
        {
            _context = context;
            _env = env;
            _logger = logger;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var noticias = await _context.Noticias
                .Include(n => n.Usuario)
                .Include(n => n.Categoria)
                .OrderByDescending(n => n.FechaPublicacion)
                .ToListAsync();

            return View(noticias);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categorias = _context.Categorias.ToList();
            return View(new Noticia
            {
                FechaPublicacion = DateTime.Now
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,Titulo,Contenido,FechaPublicacion,CategoriaId")] Noticia noticia,
            IFormFile archivoImagen)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Validar fecha de publicación
                    if (noticia.FechaPublicacion < DateTime.Now.AddMinutes(-5))
                    {
                        ModelState.AddModelError("FechaPublicacion", "La fecha de publicación no puede ser en el pasado");
                        ViewBag.Categorias = _context.Categorias.ToList();
                        return View(noticia);
                    }

                    //Imagen
                    if (archivoImagen != null && archivoImagen.Length > 0)
                    {
                        if (archivoImagen.Length > 5 * 1024 * 1024)
                        {
                            ModelState.AddModelError("ArchivoImagen", "El tamaño de la imagen no puede exceder 5MB");
                            ViewBag.Categorias = _context.Categorias.ToList();
                            return View(noticia);
                        }

                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var fileExtension = Path.GetExtension(archivoImagen.FileName).ToLower();
                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            ModelState.AddModelError("ArchivoImagen", "Solo se permiten archivos de imagen (JPG, JPEG, PNG, GIF)");
                            ViewBag.Categorias = _context.Categorias.ToList();
                            return View(noticia);
                        }

                        var uploadsFolder = Path.Combine(_env.WebRootPath, CarpetaImagenes);
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await archivoImagen.CopyToAsync(fileStream);
                        }

                        noticia.ImagenRuta = $"/{CarpetaImagenes}/{uniqueFileName}";
                    }

                    noticia.FechaRegistro = DateTime.Now;
                    noticia.UsuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    _context.Add(noticia);
                    await _context.SaveChangesAsync();

                    TempData["MensajeExito"] = "Noticia creada exitosamente";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear noticia");
                TempData["MensajeError"] = "Ocurrió un error al crear la noticia";
            }

            ViewBag.Categorias = _context.Categorias.ToList();
            return View(noticia);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var noticia = await _context.Noticias.FindAsync(id);
            if (noticia == null) return NotFound();

            if (noticia.UsuarioId != User.FindFirstValue(ClaimTypes.NameIdentifier) &&
                !User.IsInRole("Administrador"))
            {
                return Forbid();
            }

            ViewBag.Categorias = _context.Categorias.ToList();
            return View(noticia);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,Titulo,Contenido,FechaPublicacion,CategoriaId,UsuarioId,FechaRegistro,ImagenRuta")] Noticia noticia,
            IFormFile archivoImagen)
        {
            if (id != noticia.Id) return NotFound();

            try
            {
                if (ModelState.IsValid)
                {
                    var noticiaOriginal = await _context.Noticias.AsNoTracking()
                        .FirstOrDefaultAsync(n => n.Id == id);

                    if (noticiaOriginal.UsuarioId != User.FindFirstValue(ClaimTypes.NameIdentifier) &&
                        !User.IsInRole("Administrador"))
                    {
                        return Forbid();
                    }

                    //Imagen
                    if (archivoImagen != null && archivoImagen.Length > 0)
                    {
                        if (archivoImagen.Length > 5 * 1024 * 1024)
                        {
                            ModelState.AddModelError("ArchivoImagen", "El tamaño de la imagen no puede exceder 5MB");
                            ViewBag.Categorias = _context.Categorias.ToList();
                            return View(noticia);
                        }

                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var fileExtension = Path.GetExtension(archivoImagen.FileName).ToLower();
                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            ModelState.AddModelError("ArchivoImagen", "Solo se permiten archivos de imagen (JPG, JPEG, PNG, GIF)");
                            ViewBag.Categorias = _context.Categorias.ToList();
                            return View(noticia);
                        }

                        var uploadsFolder = Path.Combine(_env.WebRootPath, CarpetaImagenes);
                        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await archivoImagen.CopyToAsync(fileStream);
                        }

                        // Eliminar imagen anterior si existe
                        if (!string.IsNullOrEmpty(noticia.ImagenRuta))
                        {
                            var oldImagePath = Path.Combine(_env.WebRootPath, noticia.ImagenRuta.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                                System.IO.File.Delete(oldImagePath);
                        }

                        noticia.ImagenRuta = $"/{CarpetaImagenes}/{uniqueFileName}";
                    }
                    else
                    {
                        // Mantener la imagen existente
                        noticia.ImagenRuta = noticiaOriginal.ImagenRuta;
                    }

                    _context.Update(noticia);
                    await _context.SaveChangesAsync();

                    TempData["MensajeExito"] = "Noticia actualizada exitosamente";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!_context.Noticias.Any(e => e.Id == noticia.Id))
                    return NotFound();

                _logger.LogError(ex, "Error de concurrencia al editar noticia");
                TempData["MensajeError"] = "Ocurrió un error al actualizar la noticia";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al editar noticia");
                TempData["MensajeError"] = "Ocurrió un error al actualizar la noticia";
            }

            ViewBag.Categorias = _context.Categorias.ToList();
            return View(noticia);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var noticia = await _context.Noticias
                .Include(n => n.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (noticia == null) return NotFound();

            if (noticia.UsuarioId != User.FindFirstValue(ClaimTypes.NameIdentifier) &&
                !User.IsInRole("Administrador"))
            {
                return Forbid();
            }

            return View(noticia);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var noticia = await _context.Noticias.FindAsync(id);
            if (noticia == null) return NotFound();

            try
            {
                if (noticia.UsuarioId != User.FindFirstValue(ClaimTypes.NameIdentifier) &&
                    !User.IsInRole("Administrador"))
                {
                    return Forbid();
                }

                // Eliminar imagen asociada
                if (!string.IsNullOrEmpty(noticia.ImagenRuta))
                {
                    var imagePath = Path.Combine(_env.WebRootPath, noticia.ImagenRuta.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                        System.IO.File.Delete(imagePath);
                }

                _context.Noticias.Remove(noticia);
                await _context.SaveChangesAsync();

                TempData["MensajeExito"] = "Noticia eliminada exitosamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar noticia");
                TempData["MensajeError"] = "Ocurrió un error al eliminar la noticia";
            }

            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var noticia = await _context.Noticias
                .Include(n => n.Usuario)
                .Include(n => n.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (noticia == null) return NotFound();

            return View(noticia);
        }
    }
}