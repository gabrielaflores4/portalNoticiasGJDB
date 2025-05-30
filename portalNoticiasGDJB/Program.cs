using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using portalNoticiasGDJB.Data;

var builder = WebApplication.CreateBuilder(args);
var cadena = builder.Configuration.GetConnectionString("DefaultConnection");

string connectionResult;
try
{
    using (var conexion = new SqlConnection(cadena))
    {
        conexion.Open();
        connectionResult = "Conexión exitosa a la base de datos";
        Console.WriteLine(connectionResult);
    }
}
catch (Exception ex)
{
    connectionResult = $"Error al conectar: {ex.Message}";
    Console.WriteLine(connectionResult);
}

// Add services to the container.
builder.Services.AddDbContext<AppDb>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDb>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login"; // Asegúrate que esta ruta sea la correcta
    options.ExpireTimeSpan = TimeSpan.FromDays(14); // Mantiene sesión durante 14 días
    options.SlidingExpiration = true; // Renueva si hay actividad
});

builder.Services.AddRazorPages();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    async Task CrearRolesYAdminAsync()
    {
        string[] roles = new[] { "Admin", "Periodista", "Usuario" };

        // Crear roles si no existen
        foreach (var rol in roles)
        {
            if (!await roleManager.RoleExistsAsync(rol))
            {
                await roleManager.CreateAsync(new IdentityRole(rol));
                Console.WriteLine($"Rol {rol} creado.");
            }
        }

        // Crear usuario admin si no existe
        var adminUser = await userManager.FindByNameAsync("admin");
        if (adminUser == null)
        {
            var nuevoAdmin = new IdentityUser
            {
                UserName = "admin",
                Email = "admin@gmail.com",
                EmailConfirmed = true
            };

            var resultado = await userManager.CreateAsync(nuevoAdmin, "Admin123!");
            if (resultado.Succeeded)
            {
                Console.WriteLine("Usuario admin creado.");
                await userManager.AddToRoleAsync(nuevoAdmin, "Admin");
                Console.WriteLine("Usuario admin asignado al rol Admin.");
            }
            else
            {
                foreach (var error in resultado.Errors)
                {
                    Console.WriteLine($"Error creando usuario admin: {error.Description}");
                }
            }
        }
        else
        {
            Console.WriteLine("El usuario admin ya existe.");
        }
    }
    CrearRolesYAdminAsync().GetAwaiter().GetResult();
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapGet("/testdb", () =>
{
    return Results.Ok(connectionResult);
});

app.Run();
