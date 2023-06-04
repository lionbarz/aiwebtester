namespace AIWebTester.Controllers;

public class Utils
{
    public static Action ParseAction(string s)
    {
        var lines = s.Split('\n');

        if (lines.Length != 4)
        {
            throw new ArgumentException("Expected 4 lines");
        }
        
        var kindString = lines[0].Trim();

        if (kindString != "Click" && kindString != "Type")
        {
            throw new ArgumentException(@"The action string needs to be either ""Click"" or ""Type"".");
        }
        
        var text = lines[1].Split(':')[1].Trim();
        var targetId = lines[2].Split(':')[1].Trim();
        var explain = lines[3].Split(':')[1].Trim();

        return new Action()
        {
            Kind = kindString == "Click" ? ActionKind.Click : ActionKind.Type,
            TypeText = text,
            ElementId = targetId,
            Explain = explain
        };
    }
}