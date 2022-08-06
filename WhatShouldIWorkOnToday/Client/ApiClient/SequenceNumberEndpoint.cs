using System.Net.Http;
using System.Net.Http.Json;
using WhatShouldIWorkOnToday.Client.Models;

namespace WhatShouldIWorkOnToday.Client.ApiClient;

public class SequenceNumberEndpoint : Endpoint, ISequenceNumberEndpoint
{
    private readonly HttpClient _httpClient;

    public SequenceNumberEndpoint(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CurrentSequenceNumber> Get()
    {
        var curSeq = await _httpClient.GetFromJsonAsync<CurrentSequenceNumber>("api/SequenceNumber");
        if (curSeq is null)
        {
            throw new Exception("Failed to de-serialize work sequence list.");
        }

        return curSeq;
    }

    public async Task Put(CurrentSequenceNumber currentSequenceNumber)
    {
        using var response = await _httpClient.PutAsJsonAsync("api/SequenceNumber", currentSequenceNumber);
        CheckResponse(response);
    }
}
