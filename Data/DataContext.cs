using connections.Model;
using Microsoft.EntityFrameworkCore;
namespace connections.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

         }
         public DbSet<values> Values{get;set;}
         public DbSet<User> Users{get;set;}

         public DbSet<Photo> Photos{get;set;}
    }
}