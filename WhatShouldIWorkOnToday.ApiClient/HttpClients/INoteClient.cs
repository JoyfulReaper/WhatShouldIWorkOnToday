using WhatShouldIWorkOnToday.ApiClient.Contracts;

namespace WhatShouldIWorkOnToday.ApiClient.HttpClients;
public interface INoteClient
{
    Task<Note?> GetAsync(int noteId, CancellationToken cancellationToken = default);
    Task<List<Note>> GetByWorkItemAsync(int workItemId, CancellationToken cancellationToken = default);
    Task<Note> CreateAsync(Note note, CancellationToken cancellationToken = default);
    Task DeleteAsync(int noteId, CancellationToken cancellationToken = default);
}
