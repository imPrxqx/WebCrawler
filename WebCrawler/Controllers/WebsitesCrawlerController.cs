using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Dapper;
using MySql.Data.MySqlClient;
using WebCrawler.Models;


namespace WebCrawler.Controllers
{
    public class WebsitesCrawlerController : Controller
    {
        // GET: WebsitesCrawler/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WebsitesCrawler/Create
        [HttpPost]
        public ActionResult WebsiteRecord(WebsiteRecordModel model)
        {
            if (ModelState.IsValid)
            {

                string connectionString = "server=localhost;user=root;database=data;port=3306;password=";
                string sqlSave = "INSERT INTO node (idNode, UrlMain, OtherNodes) VALUES (@IdNode, @UrlMain, @OtherNodes)";

                SaveData(sqlSave, model, connectionString);

                string sqlLoad = "SELECT * FROM node WHERE idNode = @IdNode";
                List<WebsiteRecordModel> nodes = LoadData<WebsiteRecordModel, dynamic>(sqlLoad, model, connectionString);
                foreach (var node in nodes)
                {
                    Console.WriteLine($"ID: {node.Id}, URL: {node.Url}, Other Nodes: {node.Label}");
                }

                return RedirectToAction("Create");  
            }

            return View(model);
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
