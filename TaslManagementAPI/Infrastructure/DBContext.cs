using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaslManagementAPI.Models;

namespace TaslManagementAPI.Infrastructure
{
    public class DBContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<UserTask> Tasks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("YourConnectionStringHere");
        }
    }
}
