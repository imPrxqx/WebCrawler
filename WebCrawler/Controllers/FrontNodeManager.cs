using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WebCrawler.Models;

namespace WebCrawler.Controllers
{
    public class FrontNodeManager
    {
        public IEnumerable<Node?> ManageUrls(Uri rootUrl, string boundary, int WebsiteRecordId)
        {
            GetNodesDataFunction nodeDataFunction = new GetNodesDataFunction();
            Queue<Uri> urlQueue = new Queue<Uri>();
            HashSet<string> seenUrls = new HashSet<string>();

            if (ValidUrl(rootUrl, boundary))
            {
                urlQueue.Enqueue(rootUrl);
                seenUrls.Add(rootUrl.AbsoluteUri);
                //seenUrls.Add(RemoveFragment(rootUrl));
            }

            while (urlQueue.Count > 0)
            {
                Uri currentUrl = urlQueue.Dequeue();

                Node? currentNode = nodeDataFunction.GetNextNode(currentUrl, WebsiteRecordId);
                if (currentNode != null)
                {
                    yield return currentNode;

                    foreach (Uri nextUrl in currentNode?.nextUrls)
                    {
                        //string baseNextUrl = RemoveFragment(nextUrl);
                        string fullNextUrl = nextUrl.AbsoluteUri;

                        /*
                        if (!seenUrls.Contains(baseNextUrl) && ValidUrl(nextUrl, boundary))
                        {
                            urlQueue.Enqueue(nextUrl);
                            seenUrls.Add(baseNextUrl);
                        }*/

                        if (!seenUrls.Contains(fullNextUrl) && ValidUrl(nextUrl, boundary))
                        {
                            urlQueue.Enqueue(nextUrl);
                            seenUrls.Add(fullNextUrl);
                        }
                    }
                }
            }
        }
        /*
        private string RemoveFragment(Uri url)
        {
            return url.GetLeftPart(UriPartial.Path);
        }*/

        private bool ValidUrl(Uri url, string boundary)
        {
            return Regex.IsMatch(url.AbsoluteUri, boundary);
        }
    }
}
