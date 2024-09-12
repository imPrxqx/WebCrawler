using HtmlAgilityPack;
using WebCrawler.Models;

public class GetNodesDataFunction
{
    public Node? GetNextNode(Uri url, int WebsiteRecordId)
    {
        List<Uri> links;
        string title; 

        try
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();

                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(response.Content.ReadAsStringAsync().Result);

                links = GetAllLinks(htmlDocument, url);
                CorrectUrls(links);

                title = GetTitlePage(htmlDocument);

            }
        }
        catch
        {
            return null;
        }

        return new Node(WebsiteRecordId, url, links, title);
    }

    private string GetTitlePage(HtmlDocument htmlDocument)
    {
        HtmlNode? titleNode = htmlDocument.DocumentNode.SelectSingleNode("//title");

        if (titleNode != null)
        {
            string titlePage = titleNode.InnerText.Trim();
            return titlePage;
        }

        return string.Empty;
    }


    private List<Uri> GetAllLinks(HtmlDocument htmlDocument, Uri baseUrl)
    {
        List<Uri> links = new List<Uri>();

        HtmlNodeCollection hrefNodes = htmlDocument.DocumentNode.SelectNodes("//a[@href]");

        if (hrefNodes == null)
        {
            return links;
        }

        foreach (var node in hrefNodes)
        {
            string href = node.GetAttributeValue("href", string.Empty);

            if (string.IsNullOrWhiteSpace(href))
            {
                continue;
            }

            Uri uri;

            if (Uri.TryCreate(href, UriKind.Absolute, out uri))
            {
                if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
                {
                    links.Add(uri);
                }
            }
            else
            {
                if (Uri.TryCreate(baseUrl, href, out uri))
                {
                    if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
                    {
                        links.Add(uri);
                    }
                }
            }
        }

        return links;
    }

    private void CorrectUrls(List<Uri> links)
    {
        HashSet<string> uniqueLinks = new HashSet<string>();

        foreach (Uri link in links)
        {
            uniqueLinks.Add(link.ToString());
        }

        links.Clear();
        links.AddRange(uniqueLinks.Select(link => new Uri(link, UriKind.Absolute)).ToList());
    }
}
