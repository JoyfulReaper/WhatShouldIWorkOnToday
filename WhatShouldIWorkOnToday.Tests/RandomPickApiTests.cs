using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WhatShouldIWorkOnToday.Api;
using WhatShouldIWorkOnToday.Data;
using WhatShouldIWorkOnToday.Models;
using Xunit;

namespace WhatShouldIWorkOnToday.Tests;

public sealed class RandomPickApiTests
{
    [Fact]
    public async Task RandomPick_RequiresApiAuthentication()
    {
        using var factory = new WorkItemsApiFactory();
        using var client = factory.CreateClient(
            new Microsoft.AspNetCore.Mvc.Testing
                .WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

        using var response = await client.GetAsync(
            "/api/random-pick");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(
            "Bearer",
            response.Headers.WwwAuthenticate.Single().Scheme);
    }

    [Fact]
    public async Task RandomPick_WithNoCandidates_ReturnsNoContent()
    {
        using var factory = new WorkItemsApiFactory();
        using var client = factory.CreateAuthenticatedClient();

        using var response = await client.GetAsync(
            "/api/random-pick");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task RandomPick_WithOneCandidate_ReturnsTodoDto_AndDefaultsToUniform()
    {
        using var factory = new WorkItemsApiFactory();
        using var client = factory.CreateAuthenticatedClient();
        var todoId = await SeedTodoAsync(factory);

        using var response = await client.GetAsync(
            "/api/random-pick");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var pick = await response.Content
            .ReadFromJsonAsync<RandomPickDto>();

        Assert.NotNull(pick);
        Assert.False(pick.FavorPriority);
        Assert.Equal(todoId, pick.Todo.Id);
        Assert.Equal("API parent", pick.Todo.WorkItemName);
        Assert.Equal("API todo", pick.Todo.Task);
        Assert.Equal("High", pick.Todo.Energy);
        Assert.Equal("Long", pick.Todo.Effort);
        Assert.Equal("Low", pick.Todo.Priority);
        Assert.Null(pick.Todo.CompletedAt);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task RandomPick_ReportsRequestedMode(bool favorPriority)
    {
        using var factory = new WorkItemsApiFactory();
        using var client = factory.CreateAuthenticatedClient();
        await SeedTodoAsync(factory);

        using var response = await client.GetAsync(
            $"/api/random-pick?favorPriority={favorPriority.ToString().ToLowerInvariant()}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var pick = await response.Content
            .ReadFromJsonAsync<RandomPickDto>();

        Assert.NotNull(pick);
        Assert.Equal(favorPriority, pick.FavorPriority);
    }

    [Fact]
    public async Task RandomPick_DoesNotReturnCompletedTodo()
    {
        using var factory = new WorkItemsApiFactory();
        using var client = factory.CreateAuthenticatedClient();
        await SeedTodoAsync(
            factory,
            todoCompleted: true);

        using var response = await client.GetAsync(
            "/api/random-pick?favorPriority=true");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task RandomPick_DoesNotReturnTodoFromInactiveWorkItem(
        bool workItemCompleted,
        bool workItemArchived)
    {
        using var factory = new WorkItemsApiFactory();
        using var client = factory.CreateAuthenticatedClient();
        await SeedTodoAsync(
            factory,
            workItemCompleted: workItemCompleted,
            workItemArchived: workItemArchived);

        using var response = await client.GetAsync(
            "/api/random-pick");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private static async Task<int> SeedTodoAsync(
        WorkItemsApiFactory factory,
        bool todoCompleted = false,
        bool workItemCompleted = false,
        bool workItemArchived = false)
    {
        await using var scope = factory.Services
            .CreateAsyncScope();
        var dbContextFactory = scope.ServiceProvider
            .GetRequiredService<IDbContextFactory<AppDbContext>>();
        await using var db = await dbContextFactory
            .CreateDbContextAsync();

        var workItem = new WorkItem
        {
            Name = "API parent",
            Priority = PriorityLevel.High,
            CompletedAt = workItemCompleted
                ? DateTimeOffset.UtcNow
                : null,
            ArchivedAt = workItemArchived
                ? DateTimeOffset.UtcNow
                : null
        };
        var todo = new TodoItem
        {
            Task = "API todo",
            Energy = EnergyLevel.High,
            Effort = EffortLevel.Long,
            Priority = PriorityLevel.Low,
            CompletedAt = todoCompleted
                ? DateTimeOffset.UtcNow
                : null
        };

        workItem.Todos.Add(todo);
        db.WorkItems.Add(workItem);
        await db.SaveChangesAsync();

        return todo.Id;
    }
}
