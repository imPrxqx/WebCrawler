using Microsoft.EntityFrameworkCore;
using WebCrawler.Models;

namespace WebCrawler.GraphQl
{
    public class Query
    {
        public async Task<IEnumerable<WebPage>> GetWebsites([Service] ApplicationDbContext context)
        {
            var records = await context.WebsiteRecords.ToListAsync();


            return records.Select(record => new WebPage
            {
                Identifier = record.Id,
                Url = record.Url,
                Label = record.Label,
                Tags = record.Tags.Split('-').ToList(),
                Active = record.IsActive,
                Regexp = record.BoundaryRegExp,
            }).ToList();
        }

        
        public async Task<IEnumerable<Node>> GetNodes([ID] List<int>? webPages, [Service] ApplicationDbContext context)
        {
            var records = context.NodeRecords.AsQueryable();

            if (webPages != null && webPages.Any())
            {
                records = records.Where(node => webPages.Contains(node.WebsiteRecordId));
            }



            return records.Select(record => new Node
            {
                Title = record.title,
                Url = record.UrlMain.ToString(),
                CrawlTime = record.crawlTime,

            }).ToList();

        }
    }

    public class WebPage
    {
        [ID]
        public int Identifier { get; set; }
        public string Label { get; set; } = string.Empty; 
        public string Url { get; set; } = string.Empty; 
        public string Regexp { get; set; } = string.Empty; 
        public List<string> Tags { get; set; } = new List<string>();
        public bool Active { get; set; } 
    }

    public class Node
    {
        public string Title { get; set; } = string.Empty; 
        public string Url { get; set; } = string.Empty; 
        public string CrawlTime { get; set; } = string.Empty; 

        public List<Node> Links { get; set; } = new List<Node>(); 
        public WebPage Owner { get; set; } = new WebPage();
    }

}
