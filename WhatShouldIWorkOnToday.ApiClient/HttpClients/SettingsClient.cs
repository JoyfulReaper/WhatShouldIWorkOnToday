using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatShouldIWorkOnToday.ApiClient.HttpClients;
public class SettingsClient : ISettingsClient
{
    public Task<int> GetMaxSeqeunceNumber()
    {
        throw new NotImplementedException();
    }

    public Task<int> GetSequenceNumber()
    {
        throw new NotImplementedException();
    }

    public Task SetSequenceNumber(int sequenceNumber)
    {
        throw new NotImplementedException();
    }
}
