using System.Net.Http.Json;
using WhatShouldIWorkOnToday.Client.ApiClient.Interfaces;
using WhatShouldIWorkOnToday.Client.Models;

namespace WhatShouldIWorkOnToday.Client.ApiClient;

public class SequenceNumberEndpoint : Endpoint, ISequenceNumberEndpoint
{
    private readonly HttpClient _httpClient;

    public SequenceNumberEndpoint(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<int> GetMaxSequenceNumber()
    {
        var maxSequence = await _httpClient.GetFromJsonAsync<int>("api/SequenceNumber/MaxSequence");
        return maxSequence;
    }

    public async Task<CurrentSequenceNumber> Get()
    {
        var curSeq = await _httpClient.GetFromJsonAsync<CurrentSequenceNumber>("api/SequenceNumber");
        ThrowIfNull(curSeq);
        return curSeq!;
    }

    public async Task Put(CurrentSequenceNumber currentSequenceNumber)
    {
        using var response = await _httpClient.PutAsJsonAsync("api/SequenceNumber", currentSequenceNumber);
        CheckResponse(response);
    }
}
