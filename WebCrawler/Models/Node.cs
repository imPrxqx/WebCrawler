using System;

namespace WebCrawler.Models
{
    public struct Node
    {
        public int WebsiteRecordId { get; set; }
        public string Title { get; set; }
        public Uri url { get; set; }
        public List<Uri> nextUrls { get; set; }

        public Node(int websiteRecordId, Uri url, List<Uri> nextUrls, string Title)
        {
            this.url = url;
            this.nextUrls = nextUrls;
            this.WebsiteRecordId = websiteRecordId;
            this.Title = Title;
        }
    }
}
