using Microsoft.AspNetCore.Mvc;

namespace AIWebTester.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> Get(string url)
    {
        if (url.Length == 0) throw new Exception("Need a URL");
        
        try
        {
            var prober = await SiteProber.CreateAsync(url);
            return await prober.TakeActionAsync();
        }
        catch (Exception e)
        {
            throw new Exception("whoops", e);
        }
    }
}