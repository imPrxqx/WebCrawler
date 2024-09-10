using System.Data;
using Dapper;
using DotNetEnv;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using WebCrawler.Models;

namespace WebCrawler.Controllers
{
    public class WebsitesCrawlerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;

        private string GetConnectionString()
        {
            string databaseName = Environment.GetEnvironmentVariable("DATABASE_NAME");
            string databasePass = Environment.GetEnvironmentVariable("DATABASE_PASS");
            string databaseUser = Environment.GetEnvironmentVariable("DATABASE_USER");
            string databaseServer = Environment.GetEnvironmentVariable("DATABASE_SERVER");
            string databasePort = Environment.GetEnvironmentVariable("DATABASE_PORT");

            return $"Host={databaseServer};Port={databasePort};Username={databaseUser};Password={databasePass};Database={databaseName};";
        }

        public ActionResult CreateEditCrawler(int? id)
        {
            WebsiteRecordModel model;

            if (id.HasValue)
            {
                string sql =
                    @"
            SELECT ""Id"", ""Url"", ""BoundaryRegExp"", ""Days"", ""Hours"", ""Minutes"", ""Label"", ""IsActive"", ""Tags""
            FROM public.""WebsiteRecord""
            WHERE ""Id"" = @Id;
        ";

                model = DataAccess
                    .LoadData<WebsiteRecordModel, dynamic>(
                        sql,
                        new { Id = id.Value },
                        _connectionString
                    )
                    .FirstOrDefault();

                if (model == null)
                {
                    model = new WebsiteRecordModel();
                    model.Id = id.Value;
                }
            }
            else
            {
                model = new WebsiteRecordModel();
            }

            return View("CreateEdit", model);
        }

        [HttpPost]
public async Task<IActionResult> SaveCrawler(WebsiteRecordModel model)
{
    if (DataVerifier.WebsiteRecordVerifier(model))
    {
        try
        {
            string sql;
            if (DataVerifier.DoesWebsiteRecordExist(model.Id, _connectionString))
            {
				sql =
			@"
        UPDATE public.""WebsiteRecord"" 
        SET ""Url"" = @Url, ""BoundaryRegExp"" = @BoundaryRegExp, ""Days"" = @Days, 
            ""Hours"" = @Hours, ""Minutes"" = @Minutes, ""Label"" = @Label, 
            ""IsActive"" = @IsActive, ""Tags"" = @Tags, ""LastChange"" = @LastChange
        WHERE ""Id"" = @Id;
    ";

				DataAccess.SaveData(sql, model, _connectionString);
            }
            else
            {
				sql =
			@"
        INSERT INTO public.""WebsiteRecord"" 
        (""Url"", ""BoundaryRegExp"", ""Days"", ""Hours"", ""Minutes"", ""Label"", ""IsActive"", ""Tags"", ""LastChange"") 
        VALUES 
        (@Url, @BoundaryRegExp, @Days, @Hours, @Minutes, @Label, @IsActive, @Tags, @LastChange)
        RETURNING ""Id"";
    ";

				DataAccess.SaveData(sql, model, _connectionString);
            }
        }
        catch (Exception ex)
        {
            return Content($"Error connecting to database: {ex.Message}");
        }
    }

    return Content($"Record added successfully: {model.Id}");
}

        // GET: WebsitesCrawler/CreateCrawler
        public ActionResult CreateCrawler()
        {
            return View();
        }

        // GET: WebsitesCrawler/ShowCrawler
        public ActionResult ShowCrawler()
        {
            return View();
        }

        // GET: WebsitesCrawler/EditCrawler
        public ActionResult EditCrawler(int id)
        {
            string sql =
                @"
        SELECT ""Id"", ""Url"", ""BoundaryRegExp"", ""Days"", ""Hours"", ""Minutes"", ""Label"", ""IsActive"", ""Tags""
        FROM public.""WebsiteRecord"";
    ";

            var crawlers = DataAccess.LoadData<WebsiteRecordModel, dynamic>(
                sql,
                new { },
                _connectionString
            );

            var crawler = crawlers.FirstOrDefault(c => c.Id == id);

            if (crawler == null)
            {
                return NotFound($"Crawler with ID {id} not found.");
            }

            return View(crawler);
        }

        // GET: WebsitesCrawler/Crawlers
        public ActionResult Crawlers()
        {
            ;
            string sql =
                @"
        SELECT ""Id"", ""Url"", ""BoundaryRegExp"", ""Days"", ""Hours"", ""Minutes"", ""Label"", ""IsActive"", ""Tags""
        FROM public.""WebsiteRecord"";
    ";

            var articles = DataAccess.LoadData<WebsiteRecordModel, dynamic>(
                sql,
                new { },
                _connectionString
            );

            return View(articles);
        }

        public WebsitesCrawlerController(ApplicationDbContext context)
        {
            _context = context;
            Env.Load();
            _connectionString = GetConnectionString();
        }

        // POST: WebsitesCrawler/CreateCrawler
        [HttpPost]
        public async Task<IActionResult> CreateWebsiteRecord(WebsiteRecordModel model)
        {
            if (DataVerifier.WebsiteRecordVerifier(model))
            {
                try
                {
                    string sql =
                        @"
    INSERT INTO public.""WebsiteRecord"" 
    (""Url"", ""BoundaryRegExp"", ""Days"", ""Hours"", ""Minutes"", ""Label"", ""IsActive"", ""Tags"") 
    VALUES 
    (@Url, @BoundaryRegExp, @Days, @Hours, @Minutes, @Label, @IsActive, @Tags);
";

                    DataAccess.SaveData(sql, model, _connectionString);

                    Console.WriteLine($"Record added successfully: {model.Id}");
                    return Content($"Record added successfully: {model.Id}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Chyba při připojování k databázi: {ex.Message}");
                    return Content($"Chyba při připojování k databázi: {ex.Message}");
                }
            }
            else
            {
                return (Content($"Record badly filled: {model.Id}"));
            }
        }

        [HttpDelete]
        public IActionResult DeleteCrawler(int id)
        {
            try
            {
                string deleteNodeNeighboursSql =
                    @"
                    DELETE FROM public.""NodeNeighbours""
                    WHERE ""NodeId"" IN (SELECT ""Id"" FROM public.""Node"" WHERE ""WebsiteRecordId"" = @Id)
                    OR ""NeighbourNodeId"" IN (SELECT ""Id"" FROM public.""Node"" WHERE ""WebsiteRecordId"" = @Id);
                ";
                DataAccess.SaveData(deleteNodeNeighboursSql, new { Id = id }, _connectionString);

                string deleteNodesSql =
                    @"
                    DELETE FROM public.""Node""
                    WHERE ""WebsiteRecordId"" = @Id;
                ";
                DataAccess.SaveData(deleteNodesSql, new { Id = id }, _connectionString);

                string deleteCrawlerSql =
                    @"
                    DELETE FROM public.""WebsiteRecord""
                    WHERE ""Id"" = @Id;
                ";
                DataAccess.SaveData(deleteCrawlerSql, new { Id = id }, _connectionString);

                Console.WriteLine(
                    $"Record {id} was deleted successfully along with related nodes."
                );
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete record {id}: {ex.Message}");
                return StatusCode(500, $"Failed to delete record {id}: {ex.Message}");
            }
        }
    }
}
