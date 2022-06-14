using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HelloWorld.Database;
using Microsoft.EntityFrameworkCore;
using Dapper;

namespace HelloWorld.Controllers
{
    public class SqlInjectionController : Controller
    {
        private DemoContext _context;

        public SqlInjectionController(DemoContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("SqlInjection/SearchPersonUnsecure/{name}")]
        public async Task<IActionResult> SearchPersonUnsecure(string name)
        {
            var conn = _context.Database.GetDbConnection();
            var query = "SELECT Id, FirstName, LastName FROM Person WHERE FirstName Like '%" + name + "%'";
            IEnumerable<Person> persons;

            try
            {
                await conn.OpenAsync();
                persons = await conn.QueryAsync<Person>(query);
            }

            finally
            {
                conn.Close();
            }
            return Ok(persons);
        }
    }
}
