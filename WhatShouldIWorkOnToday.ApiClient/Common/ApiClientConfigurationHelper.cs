using System.Net.Http.Headers;
using WhatShouldIWorkOnToday.ApiClient.Common.Options;

namespace WhatShouldIWorkOnToday.ApiClient.Common;
internal static class ApiClientConfigurationHelper
{
    internal static void ConfigureHttpClient(HttpClient httpClient, WsiwotClientOptions options)
    {
        httpClient.BaseAddress = new Uri(options.BaseUrl);
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(HttpClientConstants.UserAgent, HttpClientConstants.Version));
        httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(HttpClientConstants.UserAgentComment));
    }
}
