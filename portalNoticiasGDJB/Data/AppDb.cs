using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using portalNoticiasGDJB.Models; 

namespace portalNoticiasGDJB.Data
{
    public class AppDb : IdentityDbContext<IdentityUser>
    {
        public AppDb(DbContextOptions<AppDb> options)
            : base(options)
        {
        }

        public DbSet<Noticia> Noticias { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
    }
}
