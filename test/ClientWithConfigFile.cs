using System;
using System.IO;
using NUnit.Framework;
using Ovh.Api;
using Ovh.Api.Exceptions;

namespace Ovh.Test;

[TestFixture]
public class ClientWithConfigFile
{
    public const string OvhConfigFile = ".ovh.conf";
    public const string CustomConfigFile = "some-specific-file.conf";

    [TearDown]
    public void RemoveConfigFile()
    {
        File.Delete(OvhConfigFile);
        File.Delete(CustomConfigFile);
    }

    public void CreateInvalidConfigFile()
    {
        File.WriteAllText(".ovh.conf",
            "Wrong ini" + Environment.NewLine +
            "    file!");
    }

    public void CreateConfigFileWithEndpointOnly()
    {
        File.WriteAllText(".ovh.conf",
            "[default]" + Environment.NewLine +
            "endpoint=ovh-eu");
    }

    public void CreateConfigFileWithSpecificFileName(string confFileName)
    {
        File.WriteAllText(confFileName,
            "[default]" + Environment.NewLine +
            "endpoint=ovh-eu");
    }

    public void CreateConfigFileWithAllValues()
    {
        File.WriteAllText(".ovh.conf",
            "[default]" + Environment.NewLine +
            "endpoint=ovh-eu" + Environment.NewLine +

            "[ovh-eu]" + Environment.NewLine +
            "application_key=my_app_key" + Environment.NewLine +
            "application_secret=my_application_secret" + Environment.NewLine +
            "consumer_key=my_consumer_key" + Environment.NewLine +
            "");
    }

    [Test]
    public void InvalidConfigFile()
    {
        CreateInvalidConfigFile();
        Assert.Throws(typeof(FormatException), () => new Client());
    }

    [Test]
    public void ValidConfigFileWithEndpointOnly()
    {
        CreateConfigFileWithEndpointOnly();
        Client client = new();
        Assert.Equals(client.Endpoint, "https://eu.api.ovh.com/1.0/");
    }

    [Test]
    public void ValidConfigFileWithSpecificFileName()
    {
        CreateConfigFileWithSpecificFileName(CustomConfigFile);
        Client client = new(confFileName: CustomConfigFile);
        Assert.Equals(client.Endpoint, "https://eu.api.ovh.com/1.0/");
    }

    [Test]
    public void ValidConfigFileWithAllValues()
    {
        CreateConfigFileWithAllValues();
        Client client = new();
        Assert.Equals(client.Endpoint, "https://eu.api.ovh.com/1.0/");
        Assert.Equals(client.ApplicationKey, "my_app_key");
        Assert.Equals(client.ApplicationSecret, "my_application_secret");
        Assert.Equals(client.ConsumerKey, "my_consumer_key");
    }
}