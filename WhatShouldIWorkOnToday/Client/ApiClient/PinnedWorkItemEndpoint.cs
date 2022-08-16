using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using WhatShouldIWorkOnToday.Client.Models;

namespace WhatShouldIWorkOnToday.Client.ApiClient;

public class PinnedWorkItemEndpoint : Endpoint
{ 
	private readonly HttpClient _client;

	public PinnedWorkItemEndpoint(HttpClient client)
	{
		_client = client;
	}

    public async Task<List<PinnedWorkItem>> GetAllAsync()
    {
        var pinnedWorkItems = await _client.GetFromJsonAsync<List<PinnedWorkItem>>("api/PinnedWorkItem");
        ThrowIfNull(pinnedWorkItems);

        return pinnedWorkItems!;
    }

    public async Task PinWorkItem(int workItemId)
    {
        var response = await _client.PostAsync("api/PinnedWorkItem/PinWorkItem",
            new StringContent(JsonSerializer.Serialize(workItemId), Encoding.UTF8, "application/json"));
        CheckResponse(response);
    }

    public async Task UnpinWorkItem(int workItemId)
    {
        var response = await _client.PostAsync("api/PinnedWorkItem/UnpinWorkItem",
            new StringContent(JsonSerializer.Serialize(workItemId), Encoding.UTF8, "application/json"));
        CheckResponse(response);
    }
}
