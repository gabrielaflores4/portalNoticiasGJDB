using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using portalNoticiasGDJB.Data;
using portalNoticiasGDJB.Models;

namespace portalNoticiasGDJB.Pages
{
    public class RegNoticiaModel : PageModel
    {
        private readonly AppDb _context;
        private readonly UserManager<IdentityUser> _userManager;

        public RegNoticiaModel(AppDb context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public IFormFile Imagen { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var form = Request.Form;

            string titulo = form["Titulo"];
            string contenido = form["Contenido"];
            DateTime fechaPublicacion = DateTime.Parse(form["FechaPublicacion"]);

            var userId = _userManager.GetUserId(User);

            byte[] imagenBytes = null;
            if (Imagen != null && Imagen.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await Imagen.CopyToAsync(ms);
                    imagenBytes = ms.ToArray();
                }
            }

            var noticia = new Noticia
            {
                Titulo = titulo,
                Contenido = contenido,
                FechaPublicacion = fechaPublicacion,
                FechaRegistro = DateTime.Now,
                Imagen = imagenBytes,
                UsuarioId = userId
            };

            _context.Noticias.Add(noticia);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Index");
        }
    }
}