using NUnit.Framework;

using Ovh.Api;
using Ovh.Api.Exceptions;

using System;

namespace Ovh.Test;

[TestFixture]
public class ClientWithManualParams
{
    [Test]
    public void NoParamsThrowsConfigurationKeyMissingException()
    {
        Assert.Throws<ConfigurationKeyMissingException>(() => new Client());
    }

    [Test]
    public void ValidEndpointParam()
    {
        Client client = new("ovh-eu-v1");
        Assert.That(client.Endpoint.AbsoluteUri.Trim(), Is.EqualTo("https://eu.api.ovh.com/1.0/"));
    }

    [Test]
    public void ValidParams()
    {
        Client client =
            new("ovh-eu-v1", "applicationKey", "secretKey",
                "consumerKey", defaultTimeout: TimeSpan.FromSeconds(120));
        Assert.That(client.Endpoint.AbsoluteUri.Trim(), Is.EqualTo("https://eu.api.ovh.com/1.0/"));
    }

    [Test]
    public void InvalidEndpointParamThrowsInvalidRegionException()
    {
        Assert.Throws<InvalidRegionException>(() => new Client("ovh-noWhere"));
    }
}