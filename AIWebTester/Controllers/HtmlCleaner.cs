using HtmlAgilityPack;

namespace AIWebTester.Controllers;

public class HtmlCleaner
{
    public static string CleanHtml(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        string[] selectors = new[]
        {
            "/html/head",
            "//comment()",
            "//script",
            "//style",
            "//path",
            "//img",
            "//noscript",
            "//svg"
        };

        foreach (var selector in selectors)
        {
            var nodes = doc.DocumentNode.SelectNodes(selector);

            if (nodes == null) continue;
            
            foreach (var node in nodes)
            {
                node?.Remove();
            }
        }

        foreach (var node in doc.DocumentNode.DescendantsAndSelf())
        {
            var idAttribute = node.Attributes["id"];
            node.Attributes.Remove();
            
            if (idAttribute != null)
            {
                node.Attributes.Add(idAttribute);
            }
        }

        var htmlString = doc.DocumentNode.OuterHtml;
        htmlString = htmlString.Replace("<div>", "[");
        htmlString = htmlString.Replace("</div>", "]");
        htmlString = htmlString.Replace("<span>", "[");
        htmlString = htmlString.Replace("</span>", "]");
        htmlString = htmlString.Replace("<li>", "[");
        htmlString = htmlString.Replace("</li>", "]");
        return htmlString;
    }
}