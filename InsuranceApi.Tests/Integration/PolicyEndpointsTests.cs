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

    [Fact]
    public async Task PostPolicies_ShouldReturnConflictPayload_WhenPolicyNumberAlreadyExists()
    {
        var request = new
        {
            policyNumber = "POL-001",
            subscriberName = "Alice Dupont",
            premiumAmount = 1200m,
            startDate = "2026-04-01T00:00:00Z"
        };

        await _client.PostAsJsonAsync("/policies", request);
        var secondResponse = await _client.PostAsJsonAsync("/policies", request);

        secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var content = await secondResponse.Content.ReadAsStringAsync();
        content.Should().Contain("policy_number_already_exists");
    }

    [Fact]
    public async Task PostPolicies_ShouldReturnBadRequest_WhenPremiumAmountIsInvalid()
    {
        var request = new
        {
            policyNumber = "POL-002",
            subscriberName = "Alice Dupont",
            premiumAmount = 0m,
            startDate = "2026-04-01T00:00:00Z"
        };

        var response = await _client.PostAsJsonAsync("/policies", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostPolicies_ShouldReturnBadRequest_WhenPolicyNumberIsEmpty()
    {
        var request = new
        {
            policyNumber = "",
            subscriberName = "Alice Dupont",
            premiumAmount = 1200m,
            startDate = "2026-04-01T00:00:00Z"
        };

        var response = await _client.PostAsJsonAsync("/policies", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}