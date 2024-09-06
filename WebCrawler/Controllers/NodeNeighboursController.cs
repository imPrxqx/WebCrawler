using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using WebCrawler.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebCrawler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NodeNeighboursController : ControllerBase
    {
        private readonly string _connectionString;

        public NodeNeighboursController(ApplicationDbContext context)
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

        // GET: api/<NodeNeighboursController>
        [HttpGet]
        public IEnumerable<NodeNeighbourModel> Get()
        {
            string sql =
                @"
        SELECT 
    ""NodeId"",
    ""NeighbourNodeId""
FROM 
    ""public"".""NodeNeighbours""";

            var nodes = DataAccess.LoadData<NodeNeighbourModel, dynamic>(
                sql,
                new { },
                _connectionString
            );

            return nodes;
        }

        // GET api/<NodeNeighboursController>/5
        [HttpGet("{id}")]
        public NodeNeighbourModel Get(int id)
        {
            string sql =
                @"
        SELECT 
            ""NodeId"",
            ""NeighbourNodeId""
        FROM 
            ""NodeNeighbours""
        WHERE 
            ""NodeId"" = @Id
    ";

            var node = DataAccess
                .LoadData<NodeNeighbourModel, dynamic>(sql, new { Id = id }, _connectionString)
                .FirstOrDefault();

            if (node == null)
            {
                return new NodeNeighbourModel();
            }

            return node;
        }

        // POST api/<NodeNeighboursController>
        [HttpPost]
        public void Post([FromBody] NodeNeighbourModel model)
        {
            if (!(model == null))
            {
                if (
                    DataVerifier.AreNodesFromSameWebsiteRecord(
                        model.NodeId,
                        model.NeighbourNodeId,
                        _connectionString
                    )
                )
                {
                    string sql =
                        @"
                INSERT INTO ""NodeNeighbours"" (""NodeId"", ""NeighbourNodeId"")
                VALUES (@NodeId, @NeighbourNodeId);
            ";
                    try
                    {
                        DataAccess.SaveData(
                            sql,
                            new { NodeId = model.NodeId, NeighbourNodeId = model.NeighbourNodeId },
                            _connectionString
                        );
                        Console.WriteLine($"New record created using post");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Server error: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"Record badly filled: {model.NodeId}");
                }
            }
        }

        // PUT api/<NodeNeighboursController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] NodeNeighbourModel model)
        {
            if (!(model == null))
            {
                if (
                    DataVerifier.AreNodesFromSameWebsiteRecord(
                        model.NodeId,
                        model.NeighbourNodeId,
                        _connectionString
                    )
                )
                {
                    string sql =
                        @"
            UPDATE ""NodeNeighbours""
            SET ""NeighbourNodeId"" = @NeighbourNodeId
            WHERE ""NodeId"" = @NodeId;
        ";

                    try
                    {
                        DataAccess.SaveData(
                            sql,
                            new { NodeId = model.NodeId, NeighbourNodeId = model.NeighbourNodeId },
                            _connectionString
                        );
                        Console.WriteLine("NodeNeighbour updated successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Server error: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"Node badly filled: {model.NodeId}");
                }
            }
        }

        // DELETE api/<NodeNeighboursController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            string sql =
                @"
        DELETE FROM ""NodeNeighbours""
        WHERE ""NodeId"" = @Id OR ""NeighbourNodeId"" = @Id;
    ";

            try
            {
                DataAccess.SaveData(sql, new { Id = id }, _connectionString);
                Console.WriteLine(
                    $"Neighbour with NodeId or NeighbourNodeId {id} deleted successfully."
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Server error: {ex.Message}");
            }
        }
    }
}
