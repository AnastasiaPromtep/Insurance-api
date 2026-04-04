using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace InsuranceApi.Tests.Integration;

public class PolicyEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PolicyEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostPolicies_ShouldReturnConflict_WhenPolicyNumberAlreadyExists()
    {
        var request = new
        {
            policyNumber = "POL-001",
            subscriberName = "Alice Dupont",
            premiumAmount = 1200m,
            startDate = "2026-04-01T00:00:00Z"
        };

        var firstResponse = await _client.PostAsJsonAsync("/policies", request);
        var secondResponse = await _client.PostAsJsonAsync("/policies", request);

        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}