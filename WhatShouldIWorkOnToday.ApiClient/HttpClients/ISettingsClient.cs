namespace WhatShouldIWorkOnToday.ApiClient.HttpClients;

public interface ISettingsClient
{
    Task<int> GetSequenceNumber();
    Task SetSequenceNumber(int sequenceNumber);
    Task<int> GetMaxSeqeunceNumber();

}
