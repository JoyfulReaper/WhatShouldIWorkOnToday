using WhatShouldIWorkOnToday.Client.Models;

namespace WhatShouldIWorkOnToday.Client.ApiClient;
public interface INoteEndpoint
{
    Task<List<Note>> GetAsync(int workItemId);
    Task<Note> PostNote(Note note);
}