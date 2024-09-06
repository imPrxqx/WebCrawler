using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Mysqlx.Crud;
using WebCrawler.Models;

namespace WebCrawler.GraphQl
{
    public class Query
    {
        public async Task<IEnumerable<WebPage>> GetWebsites([Service] ApplicationDbContext context)
        {
            List<WebsiteRecordModel> records = await context.WebsiteRecords.ToListAsync();


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
            List<NodeModel> records = await context.NodeRecords.ToListAsync();

            if (webPages != null && webPages.Any())
            {
                records = records.Where(node => webPages.Contains(node.WebsiteRecordId)).ToList();
            }

     
            Dictionary<int, Node> nodes = new Dictionary<int, Node>();


            foreach(var record in records)
            {
                nodes.Add(record.Id, new Node
                {
                    Title = record.Title,
                    Url = record.UrlMain,
                    CrawlTime = record.CrawlTime,
                    Owner = new WebPage
                    {
                        Identifier = record.WebsiteRecord.Id,
                        Url = record.WebsiteRecord.Url,
                        Label = record.WebsiteRecord.Label,
                        Tags = record.WebsiteRecord.Tags.Split('-').ToList(),
                        Active = record.WebsiteRecord.IsActive,
                        Regexp = record.WebsiteRecord.BoundaryRegExp,
                    },
                    Links = new List<Node>()
                });
            }

            List<NodeNeighbourModel> recordsMany = await context.NodeNeighbours.ToListAsync();

            foreach (var record in recordsMany)
            {
                nodes[record.NodeId].Links.Add(nodes[record.NeighbourNodeId]);
                nodes[record.NeighbourNodeId].Links.Add(nodes[record.NodeId]);
            }


            return records.Select(record => new Node
            {

                Title = record.Title,
                Url = record.UrlMain,
                CrawlTime = record.CrawlTime,          
                Owner = new WebPage
                {
                    Identifier = record.WebsiteRecord.Id,
                    Url = record.WebsiteRecord.Url,
                    Label = record.WebsiteRecord.Label,
                    Tags = record.WebsiteRecord.Tags.Split('-').ToList(),
                    Active = record.WebsiteRecord.IsActive,
                    Regexp = record.WebsiteRecord.BoundaryRegExp,
                },
                Links = nodes[record.Id].Links,
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
