using Microsoft.Extensions.Hosting;
using WhatShouldIWorkOnToday.ApiClient.HttpClients;

namespace NaiveTests;

internal class NaiveTestHostedClient : IHostedService
{
    private readonly ISequenceClient _sequenceClient;
    private readonly IWorkItemClient _workItemClient;

    public NaiveTestHostedClient(ISequenceClient sequenceClient, IWorkItemClient workItemClient)
    {
        _sequenceClient = sequenceClient;
        _workItemClient = workItemClient;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var seqNum = await _sequenceClient.GetSequenceNumberAsync(cancellationToken);
        var r = await _workItemClient.GetAllAsync(cancellationToken);

        foreach(var w in r)
        {
            await Console.Out.WriteLineAsync($"{w.Name}");
        }

        //var a = await _workItemClient.CreateAsync(new WhatShouldIWorkOnToday.ApiClient.Contracts.WorkItem { Name = "StupidTest" });
        var b = await _workItemClient.GetBySequenceNumberAsync(2, cancellationToken);
        b.First().Pinned = true;
        b.First().DateCompleted = DateTime.Now;
        await _workItemClient.UpdateAsync(b.First(), cancellationToken);
        var b2 = await _workItemClient.GetBySequenceNumberAsync(2, cancellationToken);

        var c = await _workItemClient.GetCompletedAsync(cancellationToken);
        await Console.Out.WriteLineAsync("Completed: " + c.Count);

        Console.WriteLine($"seqNum: {seqNum}");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
