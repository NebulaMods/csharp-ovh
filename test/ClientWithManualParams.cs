using NUnit.Framework;
using Ovh.Api;
using Ovh.Api.Exceptions;
using System;

namespace Ovh.Test;

[TestFixture]
public class ClientWithtManualParams
{
    [Test]
    public void NoParamsThrowsConfigurationKeyMissingException()
    {
        Assert.Throws<ConfigurationKeyMissingException>(() => new Client());
    }

    [Test]
    public void ValidEndpointParam()
    {
        Client client = new("ovh-eu");
        Assert.Equals(client.Endpoint, "https://eu.api.ovh.com/1.0/");
    }

    [Test]
    public void ValidParams()
    {
        Client client =
            new("ovh-eu", "applicationKey", "secretKey",
                "consumerKey", defaultTimeout: TimeSpan.FromSeconds(120));
        Assert.Equals(client.Endpoint, "https://eu.api.ovh.com/1.0/");
    }

    [Test]
    public void InvalidEndpointParamThrowsInvalidRegionException()
    {
        Assert.Throws<InvalidRegionException>(() => new Client("ovh-noWhere"));
    }
}

