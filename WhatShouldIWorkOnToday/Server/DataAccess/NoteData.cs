using WhatShouldIWorkOnToday.Server.Models;

namespace WhatShouldIWorkOnToday.Server.DataAccess;

public class NoteData : INoteData
{
    private readonly IDataAccess _dataAccess;

    public NoteData(IDataAccess dataAccess)
    {
        _dataAccess = dataAccess;
    }

    public async Task<Note?> GetAsync(int noteId)
    {
        return (await _dataAccess.LoadDataAsync<Note, dynamic>("spNote_Get", new { NoteId = noteId }, "WSIWOT"))
            .SingleOrDefault();
    }

    public async Task<List<Note>> GetByWorkItemAsync(int workItemId)
    {
        return await _dataAccess.LoadDataAsync<Note, dynamic>("spNote_GetByWorkItem", new { WorkItemId = workItemId }, "WSIWOT");
    }

    public async Task SaveAsync(Note note)
    {
        var id = await _dataAccess.SaveDataAndGetIdAsync("spNote_Upsert", new
        {
            NoteId = note.NoteId,
            WorkItemId = note.WorkItemId,
            Text = note.Text,
        }, "WSIWOT");

        note.NoteId = id;
    }
}
