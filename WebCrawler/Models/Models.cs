using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebCrawler.Models
{
    [Table("WebsiteRecord")]
    public class WebsiteRecordModel
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string BoundaryRegExp { get; set; }
        public DateTime? LastChange { get; set; }
        public int Days { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public string Label { get; set; }
        public bool IsActive { get; set; }
        public string? Tags { get; set; }
    }

    [Table("Node")]
    public class NodeModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CrawlTime { get; set; }
        public string UrlMain { get; set; }
        public int WebsiteRecordId { get; set; }
    }

    [Table("NodeNeighbour")]
    public class NodeNeighbourModel
    {
        public int NodeId { get; set; }
        public int NeighbourNodeId { get; set; }
    }
}
