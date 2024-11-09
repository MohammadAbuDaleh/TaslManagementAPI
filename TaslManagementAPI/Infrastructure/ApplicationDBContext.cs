using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using TaslManagementAPI.Models;
using TaslManagementAPI.Utilities;

namespace TaslManagementAPI.Infrastructure
{
    public class ApplicationDBContext : IdentityDbContext<User>
    {
        public DbSet<UserTask> UserTasks { get; set; }

        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultSchema(Constants.SchemaName);


            builder.Entity<UserTask>()
           .HasKey(t => t.Id);

            builder.Entity<UserTask>()
            .HasOne(t => t.User)
             .WithMany()
            .HasForeignKey(t => t.UserId);

        }
    }
}
