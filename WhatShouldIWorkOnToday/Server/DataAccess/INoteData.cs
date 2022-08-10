using WhatShouldIWorkOnToday.Server.Models;

namespace WhatShouldIWorkOnToday.Server.DataAccess;
public interface INoteData
{
    Task<List<Note>> GetAsync(int workItemId);
    Task SaveAsync(Note note);
}