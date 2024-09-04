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
        public ActionResult EditCrawler()
        {
            return View();
        }

        // GET: WebsitesCrawler/Crawlers
        public ActionResult Crawlers()
        {
            string connectionString = GetConnectionString();
            ;
            string sql =
                @"
        SELECT ""Id"", ""Url"", ""BoundaryRegExp"", ""Days"", ""Hours"", ""Minutes"", ""Label"", ""IsActive"", ""Tags""
        FROM public.""WebsiteRecord"";
    ";

            var articles = DataAccess.LoadData<WebsiteRecordModel, dynamic>(
                sql,
                new { },
                connectionString
            );

            return View(articles);
        }

        private readonly ApplicationDbContext _context;

        public WebsitesCrawlerController(ApplicationDbContext context)
        {
            _context = context;
            Env.Load();
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
            if (
                (model.Minutes < 0 || model.Minutes > 60)
                || (model.Hours < 0 || model.Hours > 24)
                || model.Days < 0
                || model.Days > 31
                || string.IsNullOrWhiteSpace(model.Url)
                || string.IsNullOrWhiteSpace(model.BoundaryRegExp)
            )
            {
                return (Content($"Record badly filled: {model.Id}"));
            }
            else
            {
                if (string.IsNullOrWhiteSpace(model.Label))
                {
                    model.Label = model.Url;
                }

                if (string.IsNullOrWhiteSpace(model.Tags))
                {
                    model.Tags = null;
                }

                try
                {
                    string connectionString = GetConnectionString();
                    string sql =
                        @"
    INSERT INTO public.""WebsiteRecord"" 
    (""Url"", ""BoundaryRegExp"", ""Days"", ""Hours"", ""Minutes"", ""Label"", ""IsActive"", ""Tags"") 
    VALUES 
    (@Url, @BoundaryRegExp, @Days, @Hours, @Minutes, @Label, @IsActive, @Tags);
";

                    DataAccess.SaveData(sql, model, connectionString);

                    Console.WriteLine($"Record added successfully: {model.Id}");
                    return Content($"Record added successfully: {model.Id}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Chyba při připojování k databázi: {ex.Message}");
                    return Content($"Chyba při připojování k databázi: {ex.Message}");
                }
            }
        }
    }
}
