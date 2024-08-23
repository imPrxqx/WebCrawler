using WebCrawler.Models;

namespace WebCrawler.Controllers
{
    public class GetNodesDataFunction
    {

        public Node GetNextNode(string url)
        {


            return new Node(
                
                "https://www.youtube.com/", 

                new List<string> { 
                    "https://www.youtube.com/watch?v=o7xxVbRWT98", 
                    "https://www.youtube.com/watch?v=58pyYC3brZc"
                }
                
                );
        }
    }
}
