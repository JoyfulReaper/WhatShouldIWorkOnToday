using WhatShouldIWorkOnToday.Auth;

namespace WhatShouldIWorkOnToday.Api;

public static partial class ApiEndpoints
{
    public static IEndpointRouteBuilder MapApiEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var api = endpoints
            .MapGroup("/api")
            .RequireAuthorization(
                ApiKeyDefaults.AuthorizationPolicy);

        api.MapGet(
            "/work-items",
            GetWorkItemsAsync);

        api.MapGet(
            "/work-items/{id:int}",
            GetWorkItemAsync);

        api.MapGet(
            "/todos",
            GetTodosAsync);

        api.MapGet(
            "/todos/{id:int}",
            GetTodoAsync);

        api.MapGet(
            "/daily-pick",
            GetDailyPickAsync);

        api.MapGet(
            "/random-pick",
            GetRandomPickAsync);

        api.MapPost(
            "/work-items",
            CreateWorkItemAsync);

        api.MapPost(
            "/work-items/{workItemId:int}/todos",
            CreateTodoAsync);

        api.MapPost(
            "/todos/bulk",
            CreateTodosBulkAsync);

        api.MapPost(
            "/work-items/bulk",
            CreateWorkItemsBulkAsync);

        return endpoints;
    }
}
