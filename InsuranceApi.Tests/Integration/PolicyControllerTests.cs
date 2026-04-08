using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace InsuranceApi.Tests.Integration;

public class PolicyControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PolicyControllerTests(CustomWebApplicationFactory factory)
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
    public async Task PostPolicies_ShouldReturnProblemDetails_WhenPolicyNumberAlreadyExists()
    {
        var request = new
        {
            policyNumber = "POL-001",
            subscriberName = "Alice Dupont",
            premiumAmount = 1200m,
            startDate = "2026-04-01T00:00:00Z"
        };

        await _client.PostAsJsonAsync("/policies", request);
        var response = await _client.PostAsJsonAsync("/policies", request);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetailsResponse>();

        problem.Should().NotBeNull();
        problem!.Title.Should().Be("Conflict");
        problem.Status.Should().Be((int)HttpStatusCode.Conflict);
        problem.Code.Should().Be("policy_number_already_exists");
        problem.Detail.Should().Contain("POL-001");
    }

    [Fact]
    public async Task PostPolicies_ShouldReturnValidationProblemDetails_WhenPremiumAmountIsInvalid()
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

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetailsResponse>();

        problem.Should().NotBeNull();
        problem!.Title.Should().Be("One or more validation errors occurred.");
        problem.Status.Should().Be((int)HttpStatusCode.BadRequest);
        problem.Errors.Should().ContainKey("PremiumAmount");
    }

    [Fact]
    public async Task PostPolicies_ShouldReturnValidationProblemDetails_WhenPolicyNumberIsEmpty()
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

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetailsResponse>();

        problem.Should().NotBeNull();
        problem!.Title.Should().Be("One or more validation errors occurred.");
        problem.Status.Should().Be((int)HttpStatusCode.BadRequest);
        problem.Errors.Should().ContainKey("PolicyNumber");
    }

    [Fact]
    public async Task PutPolicies_ShouldReturnOk_WhenRequestIsValid()
    {
        var createRequest = new
        {
            policyNumber = "POL-003",
            subscriberName = "Alice Dupont",
            premiumAmount = 1200m,
            startDate = "2026-04-01T00:00:00Z"
        };

        var createResponse = await _client.PostAsJsonAsync("/policies", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var updateRequest = new
        {
            policyNumber = "POL-003",
            subscriberName = "Alice Dupont Updated",
            premiumAmount = 1500m,
            startDate = "2026-05-01T00:00:00Z",
            endDate = (DateTime?)null,
            status = "Draft"
        };

        var updateResponse = await _client.PutAsJsonAsync("/policies/1", updateRequest);

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PutPolicies_ShouldReturnValidationProblemDetails_WhenPolicyNumberIsMissing()
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

        var response = await _client.PutAsJsonAsync("/policies/1", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetailsResponse>();

        problem.Should().NotBeNull();
        problem!.Title.Should().Be("One or more validation errors occurred.");
        problem.Status.Should().Be((int)HttpStatusCode.BadRequest);
        problem.Errors.Should().ContainKey("PolicyNumber");
    }

    [Fact]
    public async Task PutPolicies_ShouldReturnProblemDetails_WhenPolicyDoesNotExist()
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

        var response = await _client.PutAsJsonAsync("/policies/999", request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetailsResponse>();

        problem.Should().NotBeNull();
        problem!.Title.Should().Be("Not found");
        problem.Status.Should().Be((int)HttpStatusCode.NotFound);
        problem.Code.Should().Be("policy_not_found");
        problem.Detail.Should().Contain("999");
    }

    [Fact]
    public async Task DeletePolicies_ShouldReturnNoContent_WhenPolicyExists()
    {
        var createRequest = new
        {
            policyNumber = "POL-005",
            subscriberName = "Alice Dupont",
            premiumAmount = 1200m,
            startDate = "2026-04-01T00:00:00Z"
        };

        var createResponse = await _client.PostAsJsonAsync("/policies", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var deleteResponse = await _client.DeleteAsync("/policies/1");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeletePolicies_ShouldReturnProblemDetails_WhenPolicyDoesNotExist()
    {
        var response = await _client.DeleteAsync("/policies/999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetailsResponse>();

        problem.Should().NotBeNull();
        problem!.Title.Should().Be("Not found");
        problem.Status.Should().Be((int)HttpStatusCode.NotFound);
        problem.Code.Should().Be("policy_not_found");
        problem.Detail.Should().Contain("999");
    }
}