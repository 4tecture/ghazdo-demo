using System.Linq;
using System.Threading.Tasks;

namespace HelloWorld.Database
{
    public class DbInitializer
    {
        public static void Initialize(DemoContext context)
        {
            context.Database.EnsureCreated();
            if (!context.Persons.Any())
            {
                var persons = new Person[]{
                    new Person(){ Id = 1, FirstName = "John", LastName = "Doe" },
                    new Person(){ Id = 2, FirstName = "Chuck", LastName = "Norris" }
                 };

                foreach (var p in persons)
                {
                    context.Persons.Add(p);
                }
                context.SaveChanges();
            }
        }
    }
}
