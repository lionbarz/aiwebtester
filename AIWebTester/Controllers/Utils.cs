namespace AIWebTester.Controllers;

public class Utils
{
    public static Action ParseAction(string s)
    {
        var lines = s.Split('\n');

        if (lines.Length != 5)
        {
            throw new ArgumentException($"Expected 5 lines. Bad input: {s}");
        }
        
        var summary = lines[0].Trim();
        var kindString = lines[1].Split(':')[1].Trim();

        if (kindString != "Click" && kindString != "Type")
        {
            throw new ArgumentException($"The action string needs to be either \"Click\" or \"Type\". Got {kindString}");
        }
        
        var text = lines[2].Split(':')[1].Trim();
        var targetId = lines[3].Split(':')[1].Trim();
        var explain = lines[4].Split(':')[1].Trim();

        return new Action()
        {
            PageSummary = summary,
            Kind = kindString == "Click" ? ActionKind.Click : ActionKind.Type,
            TypeText = text,
            ElementId = targetId,
            Explain = explain
        };
    }
}