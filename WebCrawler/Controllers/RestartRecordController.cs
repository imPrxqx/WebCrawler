using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using WebCrawler.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebCrawler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestartRecordController : ControllerBase
    {
        private readonly string _connectionString;

        public RestartRecordController(ApplicationDbContext context)
        {
            Env.Load();
            _connectionString = GetConnectionString();
        }

        private string GetConnectionString()
        {
            string databaseName = Environment.GetEnvironmentVariable("DATABASE_NAME");
            string databasePass = Environment.GetEnvironmentVariable("DATABASE_PASS");
            string databaseUser = Environment.GetEnvironmentVariable("DATABASE_USER");
            string databaseServer = Environment.GetEnvironmentVariable("DATABASE_SERVER");
            string databasePort = Environment.GetEnvironmentVariable("DATABASE_PORT");

            return $"Host={databaseServer};Port={databasePort};Username={databaseUser};Password={databasePass};Database={databaseName};";
        }

        // GET: api/<RestartRecordController>
        [HttpGet]
        public void Get()
        {
            Console.WriteLine("this api is only for PUT");
        }

        // GET api/<RestartRecordController>/5
        [HttpGet("{id}")]
        public void Get(int id)
        {
            Console.WriteLine("this api is only for PUT");
        }

        // POST api/<RestartRecordController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
            Console.WriteLine("this api is only for PUT");
        }

        // PUT api/<RestartRecordController>/5
        [HttpPut("{id}")]
        public void Put(int id)
        {
            if (DataVerifier.DoesWebsiteRecordExist(id, _connectionString))
            {
                string sql =
                    @"
            UPDATE public.""WebsiteRecord"" 
            SET ""LastChange"" = @LastChange
            WHERE ""Id"" = @Id;
        ";
                try
                {
                    DataAccess.SaveData(
                        sql,
                        new { Id = id, LastChange = new DateTime(1, 1, 1) },
                        _connectionString
                    );
                    Console.WriteLine($"Record with Id {id} updated LastChange");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Server error: {ex.Message}");
                }
            }
        }

        // DELETE api/<RestartRecordController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            Console.WriteLine("this api is only for PUT");
        }
    }
}
