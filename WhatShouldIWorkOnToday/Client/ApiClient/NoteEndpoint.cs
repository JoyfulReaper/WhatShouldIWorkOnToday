﻿using System.Net.Http.Json;
using WhatShouldIWorkOnToday.Client.ApiClient.Interfaces;
using WhatShouldIWorkOnToday.Client.Models;

namespace WhatShouldIWorkOnToday.Client.ApiClient;

public class NoteEndpoint : Endpoint, INoteEndpoint
{
    private readonly HttpClient _httpClient;

    public NoteEndpoint(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Note>> GetAsync(int workItemId)
    {
        var notes = await _httpClient.GetFromJsonAsync<List<Note>>($"api/Note/{workItemId}");
        ThrowIfNull(notes);
        return notes!;
    }

    public async Task<Note> PostNote(Note note)
    {
        using var response = await _httpClient.PostAsJsonAsync("api/Note", note);
        CheckResponse(response);

        var noteResp = await response.Content.ReadFromJsonAsync<Note>();
        ThrowIfNull(noteResp);
        return noteResp!;
    }
}
