using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using portalNoticiasGDJB.Data;
using System;

var builder = WebApplication.CreateBuilder(args);
var cadena = builder.Configuration.GetConnectionString("DefaultConnection");

string connectionResult;
try
{
    using (var conexion = new SqlConnection(cadena))
    {
        conexion.Open();
        connectionResult = "✅ Conexión exitosa a la base de datos";
        Console.WriteLine(connectionResult);
    }
}
catch (Exception ex)
{
    connectionResult = $"❌ Error al conectar: {ex.Message}";
    Console.WriteLine(connectionResult);
}

// Add services to the container.
builder.Services.AddDbContext<AppDb>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDb>();

builder.Services.AddRazorPages();

var app = builder.Build();

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