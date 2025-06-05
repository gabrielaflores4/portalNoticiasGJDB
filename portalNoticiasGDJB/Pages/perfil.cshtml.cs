using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using portalNoticiasGDJB.Data;
using portalNoticiasGDJB.Models;
using Microsoft.EntityFrameworkCore;

namespace portalNoticiasGDJB.Pages
{
    [Authorize]
    public class perfilModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDb _context;

        public perfilModel(UserManager<IdentityUser> userManager, AppDb context)
        {
            _userManager = userManager;
            _context = context;
        }

        public string UserName { get; set; }
        public string Email { get; set; }
        public string FechaNacimiento { get; set; } = "dd/mm/aaaa";
        public bool EsAdmin { get; set; }

        public List<Noticia> NoticiasUsuario { get; set; } = new();

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                UserName = user.UserName;
                Email = user.Email;

                var roles = await _userManager.GetRolesAsync(user);
                EsAdmin = roles.Contains("admin");

                NoticiasUsuario = await _context.Noticias
                    .Where(n => n.UsuarioId == user.Id)
                    .Include(n => n.Categoria)
                    .OrderByDescending(n => n.FechaRegistro)
                    .ToListAsync();
            }
        }
    }
}

