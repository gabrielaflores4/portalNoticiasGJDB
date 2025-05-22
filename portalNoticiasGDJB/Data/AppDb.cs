using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
    
namespace portalNoticiasGDJB.Data
{
    public class AppDb : IdentityDbContext<IdentityUser>
    {
        public AppDb(DbContextOptions<AppDb> options)
            : base(options)
        {
        }
    }
}