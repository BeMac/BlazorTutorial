using BlazorTutorial.Components.Pages;
using Bunit;
using Microsoft.AspNetCore.Components.Web;

namespace UnitTests;

public class CounterTests : BunitContext
{
    [Fact]
    public void TestCounterClick()
    {
        var cut = Render<Counter>();
        cut.Find(".btn.btn-primary").Click();
        cut.Find("p[role='status']").MarkupMatches("<p role=\"status\">Current count: 1</p>");
    }
}