using System.Threading.Tasks;
using AIWebTester.Controllers;
using AIWebTester.Controllers.OpenAIModels;
using NUnit.Framework;

namespace AIWebTesterTests;

public class ProbeTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task Test1()
    {
        var prober = await SiteProber.CreateAsync("http://www.google.com");
        var html = await prober.TakeActionAsync();
        //Assert.AreEqual("blah", html);
    }
    
    [Test]
    public async Task Test2()
    {
        var response = await GptRestAdapter.CompleteChat(new Message[]
        {
            new()
            {
                Role = "user",
                Content = "How do I live in the moment?"
            } 
        }, new Function[] {});
        Assert.IsTrue(response != null);
    }
}
