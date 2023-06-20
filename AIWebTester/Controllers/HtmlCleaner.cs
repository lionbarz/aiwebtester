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

        RemoveUnnecessaryAttributes(doc);

        var htmlString = doc.DocumentNode.OuterHtml;
        //htmlString = ReplaceElementNames(htmlString);
        return htmlString;
    }

    private static void RemoveUnnecessaryAttributes(HtmlDocument doc)
    {
        foreach (var node in doc.DocumentNode.DescendantsAndSelf())
        {
            var idAttribute = node.Attributes["id"];
            var ariaLabelAttribute = node.Attributes["aria-label"];
            node.Attributes.Remove();
            
            if (idAttribute != null)
            {
                node.Attributes.Add(idAttribute);
            }
            
            if (ariaLabelAttribute != null)
            {
                node.Attributes.Add(ariaLabelAttribute);
            }
        }
    }
    
    public static string CleanHtml2(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        RemoveNonVisibleElements(doc);
        RemoveUnnecessaryAttributes(doc);
        
        var htmlString = doc.DocumentNode.OuterHtml;
        //htmlString = ReplaceElementNames(htmlString);
        return htmlString;
    }

    private static void RemoveNonVisibleElements(HtmlDocument doc)
    {
        var nodes = doc.DocumentNode.DescendantsAndSelf().ToArray();
        
        foreach (var node in nodes)
        {
            if (node == null) continue;
            
            var idAttribute = node.Attributes["mo-isvisible"];
            var isNotVisible = idAttribute == null || idAttribute.Value.ToLower() == "false";
            
            if (isNotVisible && node.NodeType != HtmlNodeType.Text)
            {
                node.Remove();
            }
        }
    }
    
    private static string ReplaceElementNames(string htmlString)
    {
        htmlString = htmlString.Replace("<div>", "[");
        htmlString = htmlString.Replace("</div>", "]");
        htmlString = htmlString.Replace("<span>", "[");
        htmlString = htmlString.Replace("</span>", "]");
        htmlString = htmlString.Replace("<li>", "[");
        htmlString = htmlString.Replace("</li>", "]");
        return htmlString;
    }
}