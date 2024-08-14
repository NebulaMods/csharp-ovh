using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Ovh.Test;

public class FakeHttpMessageHandler : HttpMessageHandler
{
    public FakeHttpMessageHandler(Dictionary<string, List<string>> expectedHeaders = null)
    { }

    public virtual HttpResponseMessage Send(HttpRequestMessage request)
    {
        throw new NotImplementedException("Now we can setup this method with our mocking framework");
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return Task.Factory.StartNew(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            var response = Send(request);
            cancellationToken.ThrowIfCancellationRequested();
            return response;
        }, cancellationToken);
    }
}