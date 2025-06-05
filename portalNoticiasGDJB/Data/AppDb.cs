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
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Guardado> Guardados { get; set; }
        public DbSet<Reaccion> Reacciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Comentario>()
                .HasOne(c => c.Noticia)
                .WithMany(n => n.Comentarios)
                .HasForeignKey(c => c.NoticiaId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Guardado>()
                .HasOne(g => g.Noticia)
                .WithMany(n => n.Guardados)
                .HasForeignKey(g => g.NoticiaId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Reaccion>()
                .HasOne(r => r.Noticia)
                .WithMany(n => n.Reacciones)
                .HasForeignKey(r => r.NoticiaId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<IdentityUser>().ToTable("AspNetUsers");
            modelBuilder.Entity<IdentityRole>().ToTable("AspNetRoles");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims");

            modelBuilder.Entity<Guardado>()
                .HasIndex(g => new { g.UsuarioId, g.NoticiaId })
                .IsUnique();

            modelBuilder.Entity<Reaccion>()
                .HasIndex(r => new { r.UsuarioId, r.NoticiaId })
                .IsUnique();

            modelBuilder.Entity<Noticia>(entity =>
            {
                entity.Property(n => n.Id).HasColumnName("Id");
                entity.Property(n => n.UsuarioId).HasColumnName("UsuarioId");
                entity.Property(n => n.CategoriaId).HasColumnName("id_categoria");
            });

            modelBuilder.Entity<Comentario>(entity =>
            {
                entity.Property(c => c.Id).HasColumnName("id_comentario");
                entity.Property(c => c.NoticiaId).HasColumnName("id_noticia");
                entity.Property(c => c.UsuarioId).HasColumnName("user_id");
            });

            //Ingresar Categorias por defecto
            modelBuilder.Entity<Categoria>().HasData(
                new Categoria { Id = 1, Nombre = "Mundo" },
                new Categoria { Id = 2, Nombre = "Economía" },
                new Categoria { Id = 3, Nombre = "Deportes" },
                new Categoria { Id = 4, Nombre = "Tecnología" },
                new Categoria { Id = 5, Nombre = "Cultura" }
            );
        }
    }
}
