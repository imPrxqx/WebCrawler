using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Mysqlx.Crud;
using WebCrawler.Models;

namespace WebCrawler.GraphQl
{
    public class Query
    {
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public async Task<IEnumerable<WebPage>> GetWebsites([Service] ApplicationDbContext context)
        {
            await _semaphore.WaitAsync();

            List<WebsiteRecordModel> records = await context.WebsiteRecords.ToListAsync();
            try
            {

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
            finally
            {
                _semaphore.Release();
            }
        }


        public async Task<IEnumerable<Node>> GetNodes([ID] List<int>? webPages, [Service] ApplicationDbContext context)
        {
            await _semaphore.WaitAsync();
            try
            {

                List<WebsiteRecordModel> recordsWebsite = await context.WebsiteRecords.ToListAsync();
                List<NodeModel> recordsNode = await context.NodeRecords.ToListAsync();
                List<NodeNeighbourModel> recordsNodeNeighbour = await context.NodeNeighbours.ToListAsync();

                Dictionary<int, Node> nodesDict = new Dictionary<int, Node>();
                Dictionary<int, WebPage> webPagesDict = new Dictionary<int, WebPage>();

                foreach (var record in recordsWebsite)
                {
                    webPagesDict.Add(record.Id,new WebPage
                    {
                        Identifier = record.Id,
                        Url = record.Url,
                        Label = record.Label,
                        Tags = record.Tags.Split('-').ToList(),
                        Active = record.IsActive,
                        Regexp = record.BoundaryRegExp,
                    });
                }

                foreach (var record in recordsNode)
                {
                    nodesDict.Add(record.Id, new Node
                    {
                        Title = record.Title,
                        Url = record.UrlMain,
                        CrawlTime = record.CrawlTime,
                        Owner = webPagesDict[record.WebsiteRecordId],
                        Links = new List<Node>()
                    });
                }

                foreach (var record in recordsNodeNeighbour)
                {
                    nodesDict[record.NodeId].Links.Add(nodesDict[record.NeighbourNodeId]);
                }

                IEnumerable<Node> filteredNodes = nodesDict.Values;

                if (webPages != null && webPages.Any())
                {
                    filteredNodes = filteredNodes.Where(node => webPages.Contains(node.Owner?.Identifier ?? 0));
                }

                return filteredNodes.ToList();
            }
            finally
            {
                _semaphore.Release();
            }
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
