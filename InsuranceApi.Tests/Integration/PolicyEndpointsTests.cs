using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using InsuranceApi.Models;

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

    [Fact]
    public async Task PutPolicies_ShouldReturnOk_WhenRequestIsValid()
    {
        // First create a policy
        var createRequest = new
        {
            policyNumber = "POL-003",
            subscriberName = "Alice Dupont",
            premiumAmount = 1200m,
            startDate = "2026-04-01T00:00:00Z"
        };

        var createResponse = await _client.PostAsJsonAsync("/policies", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Now update it
        var updateRequest = new
        {
            policyNumber = "POL-003",
            subscriberName = "Alice Dupont Updated",
            premiumAmount = 1500m,
            startDate = "2026-05-01T00:00:00Z",
            endDate = (DateTime?)null,
            status = PolicyStatus.Draft.ToString()
        };

        var updateResponse = await _client.PutAsJsonAsync("/policies/1", updateRequest);

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PutPolicies_ShouldReturnBadRequest_WhenPolicyNumberIsMissing()
    {
        var request = new
        {
            policyNumber = "",
            subscriberName = "Alice Dupont",
            premiumAmount = 1200m,
            startDate = "2026-04-01T00:00:00Z",
            endDate = (DateTime?)null,
            status = "Draft"
        };

        var response = await _client.PutAsJsonAsync("/policies", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PutPolicies_ShouldReturnNotFound_WhenPolicyDoesNotExist()
    {
        var request = new
        {
            policyNumber = "NON-EXISTENT",
            subscriberName = "Alice Dupont",
            premiumAmount = 1200m,
            startDate = "2026-04-01T00:00:00Z",
            endDate = (DateTime?)null,
            status = "Draft"
        };

        var response = await _client.PutAsJsonAsync("/policies", request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeletePolicies_ShouldReturnNoContent_WhenPolicyExists()
    {
        // First create a policy
        var createRequest = new
        {
            policyNumber = "POL-005",
            subscriberName = "Alice Dupont",
            premiumAmount = 1200m,
            startDate = "2026-04-01T00:00:00Z"
        };

        var createResponse = await _client.PostAsJsonAsync("/policies", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Now delete it
        var deleteResponse = await _client.DeleteAsync("/policies/POL-005");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeletePolicies_ShouldReturnNotFound_WhenPolicyDoesNotExist()
    {
        var response = await _client.DeleteAsync("/policies/NON-EXISTENT");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}