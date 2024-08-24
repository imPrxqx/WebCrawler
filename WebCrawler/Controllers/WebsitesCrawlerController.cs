using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
                TimeSpan crawlPeriod = model.GetPeriodicity();

                Console.WriteLine($"Crawl Periodicity: {crawlPeriod}");


                return RedirectToAction("Create");  
            }

            return View(model);
        }
    }
}
