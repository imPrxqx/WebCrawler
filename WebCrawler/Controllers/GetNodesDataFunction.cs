using HtmlAgilityPack;
using WebCrawler.Models;

public class GetNodesDataFunction
{
    public Node GetNextNode(Uri url)
    {
        List<Uri> links = new List<Uri>();

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
            }
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Error fetching data: {e.Message}");
        }

        return new Node(url, links);
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
