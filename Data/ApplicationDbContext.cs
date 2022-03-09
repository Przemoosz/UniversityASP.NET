using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FirstProject.Models;

namespace FirstProject.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<FirstProject.Models.University> University { get; set; }
        public DbSet<FirstProject.Models.Faculty> Faculty { get; set; }
        public DbSet<FirstProject.Models.Transaction> Transaction { get; set; }
    }
}