using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace portalNoticiasGDJB.Pages
{
    public class loginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public loginModel(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            public string UserName { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var result = await _signInManager.PasswordSignInAsync(Input.UserName, Input.Password, isPersistent: true, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(Input.UserName);
                var roles = await _userManager.GetRolesAsync(user);

                // Guardar rol en sesión o claims (no en localStorage)
                // Redirigir según rol, ejemplo:
                if (roles.Contains("admin"))
                    return RedirectToPage("/AdminDashboard");
                else
                    return RedirectToPage("/perfil");
            }

            ModelState.AddModelError(string.Empty, "Intento de login no válido.");
            return Page();
        }
    }
}