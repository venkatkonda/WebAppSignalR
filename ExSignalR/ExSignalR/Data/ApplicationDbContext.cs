using ExSignalR.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExSignalR.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<UserMessage> UserMessages { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}