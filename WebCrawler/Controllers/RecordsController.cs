using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using WebCrawler.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebCrawler.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RecordsController : ControllerBase
	{
		private readonly string _connectionString;

		public RecordsController(ApplicationDbContext context)
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

		// GET: api/Records
		[HttpGet]
		public IEnumerable<WebsiteRecordModel> Get()
		{
			
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

			return (articles);
		}

		// GET api/<RecordsController>/5
		[HttpGet("{id}")]
		public WebsiteRecordModel Get(int id)
		{
			string sql =
				@"
        SELECT ""Id"", ""Url"", ""BoundaryRegExp"", ""Days"", ""Hours"", ""Minutes"", ""Label"", ""IsActive"", ""Tags""
        FROM public.""WebsiteRecord""
        WHERE ""Id"" = @Id;
    ";

			var article = DataAccess
				.LoadData<WebsiteRecordModel, dynamic>(sql, new { Id = id }, _connectionString)
				.FirstOrDefault();

			if (article == null)
			{
				return new WebsiteRecordModel();
			}

			return article;
		}

		// POST api/<RecordsController>
		[HttpPost]
		public void Post([FromBody] WebsiteRecordModel model)
		{
			if (!(model == null))
			{
				if (DataVerifier.WebsiteRecordVerifier(model))
				{
					string sql =
						@"
        INSERT INTO public.""WebsiteRecord"" 
        (""Url"", ""BoundaryRegExp"", ""Days"", ""Hours"", ""Minutes"", ""Label"", ""IsActive"", ""Tags"")
        VALUES (@Url, @BoundaryRegExp, @Days, @Hours, @Minutes, @Label, @IsActive, @Tags);
    ";
					try
					{
						DataAccess.SaveData(
							sql,
							new
							{
								model.Url,
								model.BoundaryRegExp,
								model.Days,
								model.Hours,
								model.Minutes,
								model.Label,
								model.IsActive,
								model.Tags,
							},
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
					Console.WriteLine($"Record badly filled: {model.Id}");
				}
			}
		}

		// PUT api/<RecordsController>/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody] WebsiteRecordModel model)
		{
			if (!(model == null))
			{
				if (DataVerifier.WebsiteRecordVerifier(model))
				{
					string sql =
						@"
    UPDATE public.""WebsiteRecord"" 
    SET ""Url"" = @Url, 
        ""BoundaryRegExp"" = @BoundaryRegExp, 
        ""Days"" = @Days, 
        ""Hours"" = @Hours, 
        ""Minutes"" = @Minutes, 
        ""Label"" = @Label, 
        ""IsActive"" = @IsActive, 
        ""Tags"" = @Tags
    WHERE ""Id"" = @Id;
";
					try
					{
						DataAccess.SaveData(
							sql,
							new
							{
								Id = id,
								model.Url,
								model.BoundaryRegExp,
								model.Days,
								model.Hours,
								model.Minutes,
								model.Label,
								model.IsActive,
								model.Tags,
							},
							_connectionString
						);
						Console.WriteLine($"Record with Id {id} updated successfully.");
					}
					catch (Exception ex)
					{
						Console.WriteLine($"Server error: {ex.Message}");
					}
				}
				else
				{
					Console.WriteLine($"Record badly filled: {model.Id}");
				}
			}
		}

		// DELETE api/<RecordsController>/5
		[HttpDelete("{id}")]
		public void Delete(int id)
		{
			string sql =
				@"
    DELETE FROM public.""WebsiteRecord""
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
