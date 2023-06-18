using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using WhatShouldIWorkOnToday.ApiClient.Common;
using WhatShouldIWorkOnToday.ApiClient.Common.Options;

namespace WhatShouldIWorkOnToday.ApiClient.HttpClients;

public class SequenceClient : ISequenceClient
{
    private readonly WsiwotClientOptions _wsiwotOptions;
    private readonly HttpClient _httpClient;

    public SequenceClient(IOptions<WsiwotClientOptions> wsiwotOptions,
        HttpClient httpClient)
    {
        _wsiwotOptions = wsiwotOptions.Value;
        _httpClient = httpClient;

        ApiClientConfigurationHelper.ConfigureHttpClient(_httpClient, _wsiwotOptions);
    }

    public async Task<int> GetMaxSeqeunceNumberAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<int>(_httpClient.BaseAddress + "SequneceNumber/max", cancellationToken);
        return response;
    }

    public async Task<int> GetSequenceNumberAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<int>(_httpClient.BaseAddress + "SequenceNumber/", cancellationToken);
        return response;
    }

    public async Task<int> SetSequenceNumberAsync(int sequenceNumber, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PutAsync(_httpClient.BaseAddress + $"SequenceNumber/set/{sequenceNumber}", null, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = int.Parse(await response.Content.ReadAsStringAsync());
        return result;
    }
}
