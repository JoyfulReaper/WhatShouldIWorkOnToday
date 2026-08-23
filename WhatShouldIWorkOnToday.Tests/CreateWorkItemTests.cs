using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using WhatShouldIWorkOnToday.Api;
using Xunit;

namespace WhatShouldIWorkOnToday.Tests;

public sealed class CreateWorkItemTests(
    WorkItemsApiFactory factory)
    : IClassFixture<WorkItemsApiFactory>
{
    public static TheoryData<
        string,
        string?,
        string?,
        string?,
        string,
        string> InvalidRequests =>
        new()
        {
            {
                "   ",
                null,
                null,
                null,
                "name",
                "Name is required."
            },
            {
                new string('n', 201),
                null,
                null,
                null,
                "name",
                "Name cannot exceed 200 characters."
            },
            {
                "Invalid kind",
                "NotAKind",
                null,
                null,
                "kind",
                "Kind must be Project, Maintenance, Learning, Idea, or ExternalIssue."
            },
            {
                "Long description",
                null,
                new string('d', 2001),
                null,
                "description",
                "Description cannot exceed 2000 characters."
            },
            {
                "Long URL",
                null,
                null,
                new string('u', 2049),
                "url",
                "URL cannot exceed 2048 characters."
            }
        };

    [Fact]
    public async Task ValidRequest_ReturnsCreatedNormalizedDto_AndCanBeRetrieved()
    {
        using var client =
            factory.CreateAuthenticatedClient();

        var request = new CreateWorkItemRequest(
            "  Documentation refresh  ",
            "maintenance",
            "  Bring project docs up to date  ",
            "  https://example.com/docs  ",
            "HIGH");

        using var response = await client.PostAsJsonAsync(
            "/api/work-items",
            request);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var created = await response.Content
            .ReadFromJsonAsync<WorkItemDto>();

        Assert.NotNull(created);
        Assert.True(created.Id > 0);
        Assert.Equal(
            $"/api/work-items/{created.Id}",
            response.Headers.Location?.ToString());
        Assert.Equal(
            "Documentation refresh",
            created.Name);
        Assert.Equal(
            "Maintenance",
            created.Kind);
        Assert.Equal("High", created.Priority);
        Assert.Equal(
            "Bring project docs up to date",
            created.Description);
        Assert.Equal(
            "https://example.com/docs",
            created.Url);
        Assert.Equal(0, created.TodoCount);
        Assert.Equal(0, created.ActiveTodoCount);

        using var getResponse = await client.GetAsync(
            $"/api/work-items/{created.Id}");

        Assert.Equal(
            HttpStatusCode.OK,
            getResponse.StatusCode);

        var retrieved = await getResponse.Content
            .ReadFromJsonAsync<WorkItemDto>();

        Assert.NotNull(retrieved);
        Assert.Equal(created, retrieved);
    }

    [Fact]
    public async Task OmittedKind_UsesProjectDefault()
    {
        using var client =
            factory.CreateAuthenticatedClient();

        using var response = await client.PostAsJsonAsync(
            "/api/work-items",
            new
            {
                Name = "Default kind"
            });

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var created = await response.Content
            .ReadFromJsonAsync<WorkItemDto>();

        Assert.NotNull(created);
        Assert.Equal("Project", created.Kind);
        Assert.Equal("Normal", created.Priority);
        Assert.Null(created.Description);
        Assert.Null(created.Url);
    }

    [Theory]
    [MemberData(nameof(InvalidRequests))]
    public async Task InvalidRequest_ReturnsValidationProblem(
        string name,
        string? kind,
        string? description,
        string? url,
        string errorKey,
        string expectedMessage)
    {
        using var client =
            factory.CreateAuthenticatedClient();

        using var response = await client.PostAsJsonAsync(
            "/api/work-items",
            new CreateWorkItemRequest(
                name,
                kind,
                description,
                url));

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);

        var problem = await response.Content
            .ReadFromJsonAsync<HttpValidationProblemDetails>();

        Assert.NotNull(problem);
        Assert.True(
            problem.Errors.TryGetValue(
                errorKey,
                out var messages));
        Assert.Contains(
            expectedMessage,
            messages);
    }

    [Fact]
    public async Task RequestWithoutApiKey_IsRejected()
    {
        using var client = factory.CreateClient(
            new Microsoft.AspNetCore.Mvc.Testing
                .WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

        using var response = await client.PostAsJsonAsync(
            "/api/work-items",
            new CreateWorkItemRequest(
                "Unauthorized"));

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
        Assert.Equal(
            "Bearer",
            response.Headers.WwwAuthenticate
                .Single()
                .Scheme);
    }
}
