namespace WhatShouldIWorkOnToday.ApiClient.HttpClients;

public interface ISequenceClient
{
    Task<int> GetSequenceNumberAsync(CancellationToken cancellationToken = default);
    Task<int> SetSequenceNumberAsync(int sequenceNumber, CancellationToken cancellationToken = default);
    Task<int> GetMaxSeqeunceNumberAsync(CancellationToken cancellationToken = default);
}
