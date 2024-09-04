using System.ComponentModel.DataAnnotations.Schema;

namespace WebCrawler.Views
{
    [Table("Node")]
    public class NodeModel
    {
        public int Id { get; set; }
        public string title { get; set; }
        public string crawlTime { get; set; }

        public Uri UrlMain { get; set; }
        public List<Uri> UrlsNeighbours { get; set; } = new List<Uri>();

        public int WebsiteRecord { get; set; }
    }
}
