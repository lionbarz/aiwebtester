using System.Threading.Tasks;
using AIWebTester.Controllers;
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
}
