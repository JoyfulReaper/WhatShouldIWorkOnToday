using WhatShouldIWorkOnToday.Client.Models;

namespace WhatShouldIWorkOnToday.Client.ApiClient.Interfaces;
public interface INoteEndpoint
{
    Task<List<Note>> GetAsync(int workItemId);
    Task<Note> PostNote(Note note);
}