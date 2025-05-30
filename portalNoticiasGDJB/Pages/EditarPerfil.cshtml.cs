using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace portalNoticiasGDJB.Pages
{
    public class EditarPerfilModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public EditarPerfilModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public EditarDatosInputModel Input { get; set; }

        public class EditarDatosInputModel
        {
            [Required]
            public string UserName { get; set; }

            [Required, EmailAddress]
            public string Email { get; set; }

            // Para la contraseña
            [DataType(DataType.Password)]
            public string CurrentPassword { get; set; }

            [DataType(DataType.Password)]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden.")]
            public string ConfirmNewPassword { get; set; }
        }

        public async Task<IActionResult> OnPostEditarDatosAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Verificar si el nuevo nombre o email ya existen
            var userByName = await _userManager.FindByNameAsync(Input.UserName);
            if (userByName != null && userByName.Id != user.Id)
            {
                ModelState.AddModelError("Input.UserName", "El nombre de usuario ya está en uso.");
                return Page();
            }

            var userByEmail = await _userManager.FindByEmailAsync(Input.Email);
            if (userByEmail != null && userByEmail.Id != user.Id)
            {
                ModelState.AddModelError("Input.Email", "El correo electrónico ya está en uso.");
                return Page();
            }

            user.UserName = Input.UserName;
            user.Email = Input.Email;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            // Refrescar cookie de autenticación para actualizar claims, si es necesario
            await _signInManager.RefreshSignInAsync(user);

            TempData["Mensaje"] = "Datos actualizados correctamente";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCambiarContrasenaAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (string.IsNullOrEmpty(Input.CurrentPassword) || string.IsNullOrEmpty(Input.NewPassword))
            {
                ModelState.AddModelError(string.Empty, "Debe ingresar la contraseña actual y la nueva.");
                return Page();
            }

            var changePassResult = await _userManager.ChangePasswordAsync(user, Input.CurrentPassword, Input.NewPassword);

            if (!changePassResult.Succeeded)
            {
                foreach (var error in changePassResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);

            TempData["Mensaje"] = "Contraseña actualizada correctamente";
            return RedirectToPage();
        }
    }
}