using WhatShouldIWorkOnToday.Server.Models;

namespace WhatShouldIWorkOnToday.Server.DataAccess;

public class NoteData : INoteData
{
    private readonly IDataAccess _dataAccess;

    public NoteData(IDataAccess dataAccess)
    {
        _dataAccess = dataAccess;
    }

    public async Task<List<Note>> GetAsync(int workItemId)
    {
        return (await _dataAccess.LoadDataAsync<Note, dynamic>("spNote_Get", new { WorkItemId = workItemId }, "WSIWOT"))
            .ToList();
    }

    public async Task SaveAsync(Note note)
    {
        var id = await _dataAccess.SaveDataAndGetIdAsync("spNote_Upsert", new
        {
            NoteId = note.NoteId,
            WorkItemId = note.WorkItemId,
            Text = note.Text,
            DateCreated = note.DateCreated,
        }, "WSIWOT");

        note.NoteId = id;
    }
}
