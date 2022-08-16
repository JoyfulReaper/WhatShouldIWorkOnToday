using WhatShouldIWorkOnToday.Server.Models;

namespace WhatShouldIWorkOnToday.Server.DataAccess.Interfaces;
public interface INoteData
{
    Task<Note?> GetAsync(int noteId);
    Task<List<Note>> GetByWorkItemAsync(int workItemId);
    Task SaveAsync(Note note);
}