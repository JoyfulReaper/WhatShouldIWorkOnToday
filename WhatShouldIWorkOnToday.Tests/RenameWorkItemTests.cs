using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http.Json;
using WhatShouldIWorkOnToday.Api;
using Xunit;

namespace WhatShouldIWorkOnToday.Tests;

public sealed class RenameWorkItemTests(
    WorkItemsApiFactory factory)
    : IClassFixture<WorkItemsApiFactory>
{
    [Fact]
    public async Task Rename_NormalizesAndPersistsName()
    {
        using var client =
            factory.CreateAuthenticatedClient();

        using var createResponse =
            await client.PostAsJsonAsync(
                "/api/work-items",
                new
                {
                    name = "Original name"
                });

        createResponse.EnsureSuccessStatusCode();

        var created = await createResponse.Content
            .ReadFromJsonAsync<WorkItemDto>();

        Assert.NotNull(created);

        using var response =
            await client.PutAsJsonAsync(
                $"/api/work-items/{created.Id}/name",
                new RenameWorkItemRequest(
                    "  Message / Protocol Bots  "));

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var renamed = await response.Content
            .ReadFromJsonAsync<RenameWorkItemResponse>();

        Assert.NotNull(renamed);
        Assert.Equal(created.Id, renamed.Id);
        Assert.Equal(
            "Message / Protocol Bots",
            renamed.Name);

        using var getResponse =
            await client.GetAsync(
                $"/api/work-items/{created.Id}");

        var retrieved = await getResponse.Content
            .ReadFromJsonAsync<WorkItemDto>();

        Assert.NotNull(retrieved);
        Assert.Equal(
            "Message / Protocol Bots",
            retrieved.Name);
    }

    [Fact]
    public async Task Rename_BlankName_ReturnsValidationProblem()
    {
        using var client =
            factory.CreateAuthenticatedClient();

        using var createResponse =
            await client.PostAsJsonAsync(
                "/api/work-items",
                new
                {
                    name = "Original name"
                });

        var created = await createResponse.Content
            .ReadFromJsonAsync<WorkItemDto>();

        Assert.NotNull(created);

        using var response =
            await client.PutAsJsonAsync(
                $"/api/work-items/{created.Id}/name",
                new RenameWorkItemRequest("   "));

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);

        var problem = await response.Content
            .ReadFromJsonAsync<HttpValidationProblemDetails>();

        Assert.NotNull(problem);
        Assert.Contains(
            "Name is required.",
            problem.Errors["name"]);
    }

    [Fact]
    public async Task Rename_MissingWorkItem_ReturnsNotFound()
    {
        using var client =
            factory.CreateAuthenticatedClient();

        using var response =
            await client.PutAsJsonAsync(
                "/api/work-items/2147483647/name",
                new RenameWorkItemRequest(
                    "Does not matter"));

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }
}