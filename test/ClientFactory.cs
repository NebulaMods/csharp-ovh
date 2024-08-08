using System;
using System.Net.Http;
using Ovh.Api;

namespace Ovh.Test;

public static class ClientFactory
{


    public static Client GetClient(FakeHttpMessageHandler handler, bool withConsumerKey = true, TimeSpan? timeout = null)
    {
        return withConsumerKey
            ? new Client(Constants.ENDPOINT, Constants.APPLICATION_KEY, Constants.APPLICATION_SECRET, Constants.CONSUMER_KEY, httpClient: new HttpClient(handler), defaultTimeout: timeout)
            : new Client(Constants.ENDPOINT, Constants.APPLICATION_KEY, Constants.APPLICATION_SECRET, httpClient: new HttpClient(handler), defaultTimeout: timeout);
    }

}