using Microsoft.EntityFrameworkCore;

namespace HelloWorld.Database
{
    public class DemoContext : DbContext
    {
        public DemoContext(DbContextOptions<DemoContext> options) : base(options)
        {
        }

        public DbSet<Person> Persons { get; set; }
    }
}