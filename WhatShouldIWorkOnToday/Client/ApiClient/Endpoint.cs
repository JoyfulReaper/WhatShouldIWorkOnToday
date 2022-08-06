namespace WhatShouldIWorkOnToday.Client.ApiClient;

public class Endpoint
{
    protected virtual void CheckResponse(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"API Response was not successful: {response.StatusCode}");
        }
    }
}
