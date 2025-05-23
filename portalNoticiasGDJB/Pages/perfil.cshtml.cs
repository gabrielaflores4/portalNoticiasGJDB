using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace portalNoticiasGDJB.Pages
{
    public class perfilModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public perfilModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public string UserName { get; set; }
        public string Email { get; set; }
        public string FechaNacimiento { get; set; } = "dd/mm/aaaa";

        public bool EsAdmin { get; set; }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                UserName = user.UserName;
                Email = user.Email;

                var roles = await _userManager.GetRolesAsync(user);
                EsAdmin = roles.Contains("admin");
            }
        }
    }
}
