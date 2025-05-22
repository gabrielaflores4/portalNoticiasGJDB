using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace portalNoticiasGDJB.Pages
{
    public class SignUpModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public SignUpModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Propiedad que enlaza los datos del formulario
        [BindProperty]
        public InputModel Input { get; set; }

        // Modelo para datos que recibimos del formulario
        public class InputModel
        {
            [Required(ErrorMessage = "El nombre es obligatorio")]
            [Display(Name = "Nombre")]
            public string UserName { get; set; }

            [Required(ErrorMessage = "El correo es obligatorio")]
            [EmailAddress(ErrorMessage = "El correo no es válido")]
            [Display(Name = "Correo Electrónico")]
            public string Email { get; set; }

            [Required(ErrorMessage = "La contraseña es obligatoria")]
            [DataType(DataType.Password)]
            [Display(Name = "Contraseña")]
            public string Password { get; set; }

            [Required(ErrorMessage = "Confirma la contraseña")]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
            [Display(Name = "Confirmar Contraseña")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet()
        {
            // Simplemente muestra la página (GET)
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Si hay errores de validación, recarga la página con mensajes
                return Page();
            }

            // Crear un nuevo usuario IdentityUser
            var user = new IdentityUser
            {
                UserName = Input.UserName,
                Email = Input.Email
            };

            // Intentar registrar el usuario con la contraseña
            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                // Si el registro fue exitoso, iniciar sesión automáticamente
                await _signInManager.SignInAsync(user, isPersistent: false);

                // Redirigir a la página principal o la que quieras
                return RedirectToPage("/Index");
            }

            // Si hubo errores, agregarlos al ModelState para mostrarlos en la vista
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            // Mostrar la página con errores
            return Page();
        }
    }
}
