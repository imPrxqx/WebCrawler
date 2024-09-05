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
        // GET: WebsitesCrawler/CreateCrawler
        public ActionResult CreateCrawler()
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

        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;

        public WebsitesCrawlerController(ApplicationDbContext context)
        {
            _context = context;
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
    }
}
