using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WebCrawler.Models;

namespace WebCrawler.Controllers
{
    public class FrontNodeManager
    {
        public IEnumerable<Node?> ManageUrls(Uri rootUrl, string boundary)
        {
            GetNodesDataFunction nodeDataFunction = new GetNodesDataFunction();
            Queue<Uri> urlQueue = new Queue<Uri>();
            HashSet<Uri> seenUrls = new HashSet<Uri>();

            if (ValidUrl(rootUrl, boundary))
            {
                urlQueue.Enqueue(rootUrl);
                seenUrls.Add(rootUrl);
            }

            while (urlQueue.Count > 0)
            {
                Uri currentUrl = urlQueue.Dequeue();

                Node? currentNode = nodeDataFunction.GetNextNode(currentUrl);
                if (currentNode != null)
                {
                    yield return currentNode; 

                    foreach (Uri nextUrl in currentNode?.nextUrls)
                    {
                        if (!seenUrls.Contains(nextUrl) && ValidUrl(nextUrl, boundary))
                        {
                            urlQueue.Enqueue(nextUrl);
                            seenUrls.Add(nextUrl);
                        }
                    }
                }
            }
        }

        private bool ValidUrl(Uri url, string boundary)
        {
            return Regex.IsMatch(url.AbsoluteUri, boundary);
        }
    }
}
