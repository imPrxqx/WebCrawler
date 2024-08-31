using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Dapper;
using MySql.Data.MySqlClient;
using WebCrawler.Models;
using Microsoft.EntityFrameworkCore;


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
			var articles = new List<WebsiteRecordModel>
			{
				new WebsiteRecordModel {Id = 1},
				new WebsiteRecordModel{Id = 2},new WebsiteRecordModel{Id = 3},new WebsiteRecordModel{Id = 4},                new WebsiteRecordModel{Id = 2},new WebsiteRecordModel{Id = 3},new WebsiteRecordModel{Id = 4},
				new WebsiteRecordModel{Id = 2},new WebsiteRecordModel{Id = 3},new WebsiteRecordModel{Id = 4},
				new WebsiteRecordModel{Id = 2},new WebsiteRecordModel{Id = 3},new WebsiteRecordModel{Id = 4},
				new WebsiteRecordModel{Id = 2},new WebsiteRecordModel{Id = 3},new WebsiteRecordModel{Id = 4},
				new WebsiteRecordModel{Id = 2},new WebsiteRecordModel{Id = 3},new WebsiteRecordModel{Id = 4},
				new WebsiteRecordModel{Id = 2},new WebsiteRecordModel{Id = 3},new WebsiteRecordModel{Id = 4},
				new WebsiteRecordModel{Id = 2},new WebsiteRecordModel{Id = 3},new WebsiteRecordModel{Id = 4},
				new WebsiteRecordModel{Id = 2},new WebsiteRecordModel{Id = 3},new WebsiteRecordModel{Id = 4},
				new WebsiteRecordModel{Id = 2},new WebsiteRecordModel{Id = 3},new WebsiteRecordModel{Id = 4},
				new WebsiteRecordModel{Id = 2},new WebsiteRecordModel{Id = 3},new WebsiteRecordModel{Id = 4}
			};


			return View(articles);
		}

		private readonly ApplicationDbContext _context;

		public WebsitesCrawlerController(ApplicationDbContext context)
		{
			_context = context;
		}



		// POST: WebsitesCrawler/CreateCrawler
		[HttpPost]
		public async Task<IActionResult> CreateWebsiteRecord(WebsiteRecordModel model)
		{



			try
			{
				var testRecord = await _context.WebsiteRecords.FirstOrDefaultAsync(); 

				if (testRecord != null)
				{
					Console.WriteLine($"Úspěšně připojeno k databázi. Nalezený záznam: {testRecord.Id}");
					return Content($"Úspěšně připojeno k databázi. Nalezený záznam: {testRecord.Id}");
				}
				else
				{
					Console.WriteLine("Úspěšně připojeno k databázi, ale žádné záznamy nebyly nalezeny.");
					return Content("Úspěšně připojeno k databázi, ale žádné záznamy nebyly nalezeny.");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Chyba při připojování k databázi: {ex.Message}");
				return Content($"Chyba při připojování k databázi: {ex.Message}");
			}
		}



		public List<T> LoadData<T, U>(string sql, U parameters, string connectionString)
		{
			using (IDbConnection connection = new MySqlConnection(connectionString))
			{
				List<T> row = connection.Query<T>(sql, parameters).ToList();

				return row;

			}
		}

		public void SaveData<T>(string sql, T parameters, string connectionString)
		{
			using (IDbConnection connection = new MySqlConnection(connectionString))
			{
				connection.Execute(sql, parameters);
			}
		}
	}
}
