using Boilerplate.Authentication.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Boilerplate.Authentication.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }

        public DataContext(DbContextOptions<DataContext> options) 
            : base(options)
        {
        }
    }
}
