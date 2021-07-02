using Microsoft.EntityFrameworkCore;
using Appets.Domain;

namespace Appets.DataAccess
{
    public class AppetsContext : DbContext
    {
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Session> Sessions { get; set; }

        public AppetsContext(DbContextOptions options) : base(options) { }
    }
}