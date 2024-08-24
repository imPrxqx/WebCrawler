using System;

namespace WebCrawler.Models
{
    public struct Node
    {
        public Uri url { get; set; }
        public List<Uri> nextUrls { get; set; }

        public Node(Uri url, List<Uri> nextUrls)
        {
            this.url = url;
            this.nextUrls = nextUrls;
        } 
    }
}
