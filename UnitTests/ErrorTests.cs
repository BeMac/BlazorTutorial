using Bunit;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using BlazorTutorial.Components.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace UnitTests;

/// <summary>
/// Unit tests for the Error.razor page component.
/// </summary>
public class ErrorPageTests : BunitContext
{

    /// <summary>
    /// Verifies that the Request ID is displayed when Activity.Current.Id is available.
    /// </summary>
    [Fact]
    public void ErrorPage_DisplaysRequestId_WhenActivityCurrentIdIsPresent()
    {
        // Arrange

        // Start a new Activity to simulate Activity.Current being set
        using var activity = new Activity("TestActivity").SetIdFormat(ActivityIdFormat.W3C).Start();
        activity.SetParentId("parent-span-id"); // Set a parent ID to ensure a valid ID format
        activity.ActivityTraceFlags = ActivityTraceFlags.Recorded; // Often set for trace IDs
        
        // Ensure no HttpContext is cascaded to prioritize Activity.Current
        Services.RemoveAll<HttpContext>(); 

        // Act
        var cut = Render<Error>();

        // Assert
        // Check that the Request ID paragraph is rendered and contains the correct ID from Activity.Current
        var requestIdElement = cut.Find("p:contains('Request ID:') code");
        Assert.NotNull(requestIdElement);
        requestIdElement.MarkupMatches($"<code>{activity.Id}</code>");

        // The 'using' statement for 'activity' ensures it's stopped and disposed, cleaning up Activity.Current.
    }

    /// <summary>
    /// Verifies that the Request ID is NOT displayed when neither HttpContext.TraceIdentifier
    /// nor Activity.Current.Id is available.
    /// </summary>
    [Fact]
    public void ErrorPage_DoesNotDisplayRequestId_WhenNoIdIsPresent()
    {
        // Arrange
        // Ensure no HttpContext is cascaded
        Services.RemoveAll<HttpContext>();
        
        // Ensure Activity.Current is null before rendering (it should be by default in tests
        // unless explicitly set by a previous test without proper cleanup)
        Assert.Null(Activity.Current); 

        // Act
        var cut = Render<Error>();

        // Assert
        // The paragraph containing "Request ID:" should not be found if no ID is present
        Assert.Throws<ElementNotFoundException>(() => cut.Find("p:contains('Request ID:')"));
    }
}