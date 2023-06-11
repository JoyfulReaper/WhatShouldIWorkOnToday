using Microsoft.Extensions.Hosting;
using WhatShouldIWorkOnToday.ApiClient.HttpClients;

namespace NaiveTests;

internal class NaiveTestHostedClient : IHostedService
{
    private readonly ISequenceClient _sequenceClient;

    public NaiveTestHostedClient(ISequenceClient sequenceClient)
    {
        _sequenceClient = sequenceClient;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var seqNum = await _sequenceClient.GetSequenceNumberAsync(cancellationToken);
        Console.WriteLine($"seqNum: {seqNum}");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
