using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using WebCrawler.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebCrawler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NodesController : ControllerBase
    {
        private readonly string _connectionString;

        public NodesController(ApplicationDbContext context)
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

        // GET: api/<NodesController>
        [HttpGet]
        public IEnumerable<NodeModel> Get()
        {
            string sql =
                @"
                SELECT 
                    ""Id"", 
                    ""Title"", 
                    ""CrawlTime"", 
                    ""UrlMain"", 
                    ""WebsiteRecordId""
                FROM ""Node"";
            ";

            var nodes = DataAccess.LoadData<NodeModel, dynamic>(sql, new { }, _connectionString);

            return nodes;
        }

        // GET api/<NodesController>/5
        [HttpGet("{id}")]
        public NodeModel Get(int id)
        {
            string sql =
                @"
        SELECT 
            ""Id"", 
            ""Title"", 
            ""CrawlTime"", 
            ""UrlMain"", 
            ""WebsiteRecordId""
        FROM ""Node""
        WHERE ""Id"" = @Id;
    ";

            var node = DataAccess
                .LoadData<NodeModel, dynamic>(sql, new { Id = id }, _connectionString)
                .FirstOrDefault();

            if (node == null)
            {
                return new NodeModel();
            }

            return node;
        }

        // POST api/<NodesController>
        [HttpPost]
        public void Post([FromBody] NodeModel model)
        {
            if (!(model == null))
            {
                if (DataVerifier.DoesWebsiteRecordExist(model.WebsiteRecordId, _connectionString))
                {
                    string sql =
                        @"
        INSERT INTO ""Node"" 
        (""Title"", ""CrawlTime"", ""UrlMain"", ""WebsiteRecordId"") 
        VALUES 
        (@Title, @CrawlTime, @UrlMain, @WebsiteRecordId);
    ";
                    try
                    {
                        Console.WriteLine(
                            $"Attempting to insert Node with Title: {model.Title}, CrawlTime: {model.CrawlTime}, UrlMain: {model.UrlMain}, WebsiteRecordId: {model.WebsiteRecordId}"
                        );
                        DataAccess.SaveData(
                            sql,
                            new
                            {
                                Title = model.Title,
                                CrawlTime = model.CrawlTime,
                                UrlMain = model.UrlMain,
                                WebsiteRecordId = model.WebsiteRecordId,
                            },
                            _connectionString
                        );
                        Console.WriteLine($"New Node created using post");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Server error: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"Node badly filled: {model.Id}");
                }
            }
        }

        // PUT api/<NodesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] NodeModel model)
        {
            if (!(model == null))
            {
                if (DataVerifier.DoesWebsiteRecordExist(model.WebsiteRecordId, _connectionString))
                {
                    string sql =
                        @"
        UPDATE ""Node"" 
        SET 
            ""Title"" = @Title, 
            ""CrawlTime"" = @CrawlTime, 
            ""UrlMain"" = @UrlMain, 
            ""WebsiteRecordId"" = @WebsiteRecordId
        WHERE 
            ""Id"" = @Id;
    ";
                    try
                    {
                        DataAccess.SaveData(
                            sql,
                            new
                            {
                                Id = id,
                                Title = model.Title,
                                CrawlTime = model.CrawlTime,
                                UrlMain = model.UrlMain,
                                WebsiteRecordId = model.WebsiteRecordId,
                            },
                            _connectionString
                        );

                        Console.WriteLine($"Node with Id {id} updated successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Server error: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"NODE badly filled: {model.Id}");
                }
            }
        }

        // DELETE api/<NodesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            string sql =
                @"
        DELETE FROM ""Node"" 
        WHERE ""Id"" = @Id;
    ";

            try
            {
                DataAccess.SaveData(sql, new { Id = id }, _connectionString);

                Console.WriteLine($"Record with Id {id} deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Server error: {ex.Message}");
            }
        }
    }
}
