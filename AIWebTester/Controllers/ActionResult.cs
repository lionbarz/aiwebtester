namespace AIWebTester.Controllers;

public class ActionResult
{
    public string BeforeScreenshotPath { get; set; }
    public string AfterScreenshotPath { get; set; }
    public string Expected { get; set; }
    
    /// <summary>
    /// The action that was taken.
    /// </summary>
    public Action Action { get; set; }

    public string AfterScreenshotBytes { get; set; }
    public string BeforeScreenshotBytes { get; set; }
}