using System.Net.Http.Json;
using WhatShouldIWorkOnToday.Client.ApiClient.Interfaces;
using WhatShouldIWorkOnToday.Client.Models;

namespace WhatShouldIWorkOnToday.Client.ApiClient;

public class TodoItemEndpoint : Endpoint, ITodoItemEndpoint
{
	private readonly HttpClient _httpClient;

	public TodoItemEndpoint(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task<IEnumerable<TodoItem>> GetAsync(int workItemId)
	{
		var items = await _httpClient.GetFromJsonAsync<IEnumerable<TodoItem>>($"/api/TodoItem/{workItemId}");
		ThrowIfNull(items);

		return items!;
	}

	public async Task CompleteAsync(int todoItemId)
	{
		using var response = await _httpClient.PostAsJsonAsync($"/api/TodoItem/Complete/{todoItemId}", todoItemId);
		CheckResponse(response);
	}

	public async Task<TodoItem> PostAsync(TodoItem item)
	{
		using var response = await _httpClient.PostAsJsonAsync("/api/TodoItem", item);
		CheckResponse(response);

		var itemResp = await response.Content.ReadFromJsonAsync<TodoItem>();
		ThrowIfNull(itemResp);

		return itemResp!;
	}
}
