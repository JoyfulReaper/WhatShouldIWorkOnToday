namespace WhatShouldIWorkOnToday.Client;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

public class BasicAuthenticationService
{
    private readonly HttpClient httpClient;
    private readonly NavigationManager navigationManager;

    public BasicAuthenticationService(HttpClient httpClient, NavigationManager navigationManager)
    {
        this.httpClient = httpClient;
        this.navigationManager = navigationManager;
    }

    public async Task<bool> Authenticate(string username, string password)
    {
        // Create the Basic Authentication header
        var authHeader = GetBasicAuthenticationHeader(username, password);

        // Set the Authorization header on the HttpClient
        httpClient.DefaultRequestHeaders.Authorization = authHeader;

        try
        {
            // Send a test request to check if the authentication is successful
            // Replace "/api/test" with the actual endpoint you want to test
            var response = await httpClient.GetAsync("/api/v1/WorkItem/2");
            if (response.IsSuccessStatusCode)
            {
                return true; // Authentication successful
            }
        }
        catch
        {
            // Handle any exceptions (e.g., server not reachable)
        }

        return false; // Authentication failed
    }

    private AuthenticationHeaderValue GetBasicAuthenticationHeader(string username, string password)
    {
        var token = $"{username}:{password}";
        var tokenBytes = System.Text.Encoding.UTF8.GetBytes(token);
        var base64Token = System.Convert.ToBase64String(tokenBytes);
        return new AuthenticationHeaderValue("Basic", base64Token);
    }
}