namespace WebCrawler.Models
{
    public struct Node
    {
        public string url { get; set; }
        public List<string> nextUrls { get; set; }

        public Node(string url, List<string> nextUrls)
        {
            this.url = url;
            this.nextUrls = nextUrls;
        } 
    }
}
