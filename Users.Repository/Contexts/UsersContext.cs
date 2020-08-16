using Microsoft.EntityFrameworkCore;
using Users.Repository.Entities;

namespace Users.Repository.Contexts
{
    public class UsersContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public UsersContext(DbContextOptions<UsersContext> options)
            : base(options)
        {
        }
    }
}