using Ovh.Api;
using System;
using NUnit.Framework;

namespace Ovh.Test;

[TestFixture]
public class ClientWithEnvironmentParams
{
    [SetUp]
    public void AddEnpointToEnv()
    {
        Environment.SetEnvironmentVariable("OVH_ENDPOINT", "ovh-eu", EnvironmentVariableTarget.Process);
    }

    [TearDown]
    public void RemoveEnvVariables()
    {
        Environment.SetEnvironmentVariable("OVH_ENDPOINT", null, EnvironmentVariableTarget.Process);
        Environment.SetEnvironmentVariable("OVH_APPLICATION_KEY", null, EnvironmentVariableTarget.Process);
        Environment.SetEnvironmentVariable("OVH_APPLICATION_SECRET", null, EnvironmentVariableTarget.Process);
        Environment.SetEnvironmentVariable("OVH_CONSUMER_KEY", null, EnvironmentVariableTarget.Process);
    }

    public void AddOtherParamsToEnv()
    {
        Environment.SetEnvironmentVariable("OVH_APPLICATION_KEY", "my_app_key", EnvironmentVariableTarget.Process);
        Environment.SetEnvironmentVariable("OVH_APPLICATION_SECRET", "my_application_secret", EnvironmentVariableTarget.Process);
        Environment.SetEnvironmentVariable("OVH_CONSUMER_KEY", "my_consumer_key", EnvironmentVariableTarget.Process);
    }

    [Test]
    public void ValidEndpoint()
    {
        Client client = new();
        Assert.Equals(client.Endpoint, "https://eu.api.ovh.com/1.0/");
    }

    [Test]
    public void AllParams()
    {
        AddOtherParamsToEnv();
        Client client = new();
        Assert.Equals(client.Endpoint, "https://eu.api.ovh.com/1.0/");
        Assert.Equals(client.ApplicationKey, "my_app_key");
        Assert.Equals(client.ApplicationSecret, "my_application_secret");
        Assert.Equals(client.ConsumerKey, "my_consumer_key");
    }
}

