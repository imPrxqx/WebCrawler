using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WebCrawler.Models;

namespace WebCrawler.Controllers
{
    public class FrontNodeManager
    {
        public List<Node?> ManageUrls(Uri rootUrl, string boundary)
        {
            GetNodesDataFunction nodeDataFunction = new GetNodesDataFunction();
            Queue<Uri> urlQueue = new Queue<Uri>();
            HashSet<Uri> SeenUrls = new HashSet<Uri>();
            List<Node?> nodeList = new List<Node?>();

            if (validUrl(rootUrl, boundary))
            {
                urlQueue.Enqueue(rootUrl);
                SeenUrls.Add(rootUrl);
            }

            while (urlQueue.Count > 0)
            {
                Uri currentUrl = urlQueue.Dequeue();

                Node? currentNode = nodeDataFunction.GetNextNode(currentUrl);
                if(currentNode == null)
                {
                    continue;
                }
                nodeList.Add(currentNode);

                foreach (Uri nextUrl in currentNode?.nextUrls)
                {
                    if (!SeenUrls.Contains(nextUrl) && validUrl(nextUrl, boundary))
                    {
                        urlQueue.Enqueue(nextUrl);
                        SeenUrls.Add(nextUrl);
                    }
                }
            }
            Console.WriteLine("finished");
            return (nodeList);
        }

        private bool validUrl(Uri url, string boundary)
        {
            if (Regex.IsMatch(url.AbsoluteUri, boundary))
            {
                return true;
            }
            return false;
        }
    }
}
