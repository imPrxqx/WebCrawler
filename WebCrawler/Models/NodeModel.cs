using System.ComponentModel.DataAnnotations.Schema;

namespace WebCrawler.Models
{
    [Table("Node")]
    public class NodeModel
    {
        public int Id { get; set; }
        public string title { get; set; }
        public string crawlTime { get; set; }

        public Uri UrlMain { get; set; }
        public List<NodeModel> UrlsNeighbours { get; set; } = new List<NodeModel>();

        public int WebsiteRecordId { get; set; }

        [ForeignKey("WebsiteRecordId")]
        public WebsiteRecordModel WebsiteRecord { get; set; }
    }
}
