using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Users.Repository.Contexts;

namespace Users.Repository.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddUsersRepository(this IServiceCollection collection)
        {
            collection.AddDbContext<UsersContext>(options => options.UseInMemoryDatabase(databaseName: "Users"));

            return collection;
        }
    }
}