using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace InsuranceApi.Tests;

public class PolicyEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PolicyEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetPolicies_ShouldReturnSuccessStatusCode()
    {
        var response = await _client.GetAsync("/policies");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}