using FakeItEasy;

using NUnit.Framework;

using Ovh.Api;
using Ovh.Api.Testing;
using Ovh.Test.Models;

using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Ovh.Test;

[TestFixture]
public class GetRequests
{
    private static long currentClientTimestamp = 1566485765;
    private static long currentServerTimestamp = 1566485767;
    private static DateTimeOffset currentDateTime = DateTimeOffset.FromUnixTimeSeconds(currentClientTimestamp);
    private static ITimeProvider timeProvider = A.Fake<ITimeProvider>();

    public GetRequests()
    {
        A.CallTo(() => timeProvider.UtcNow).Returns(currentDateTime);
    }

    public static void MockAuthTimeCallWithFakeItEasy(FakeHttpMessageHandler fake)
    {
        A.CallTo(() =>
            fake.Send(A<HttpRequestMessage>.That.Matches(
                r => r.RequestUri.ToString().Contains("/auth/time"))))
            .Returns(Responses.Get.time_message);
    }

    [Test]
    public async Task GET_auth_time()
    {
        var testHandler = A.Fake<FakeHttpMessageHandler>(a => a.CallsBaseMethods());
        MockAuthTimeCallWithFakeItEasy(testHandler);

        var httpClient = new HttpClient(testHandler);
        var c = new Client("ovh-eu-v1", httpClient: httpClient).AsTestable(timeProvider);

        Assert.That(2, Is.EqualTo(await c.GetTimeDelta()));
    }

    [Test]
    [TestCase("/me", "https://eu.api.ovh.com/1.0/me", "$1$dfe0b86bf2ab0d9eb3f785dc1ab00de58984d80c")]
    [TestCase("/v1/me", "https://eu.api.ovh.com/v1/me", "$1$b6849b8a25d6bc46c6ad1dfb0fc67d07db9553a3")]
    [TestCase("/v2/me", "https://eu.api.ovh.com/v2/me", "$1$291bb7bdbef11b1050200a109a4fe5109ed96cdd")]
    public async Task GET_me_as_string(string call, string called, string sig)
    {
        var fake = A.Fake<FakeHttpMessageHandler>(a => a.CallsBaseMethods());
        MockAuthTimeCallWithFakeItEasy(fake);

        A.CallTo(() =>
            fake.Send(A<HttpRequestMessage>.That.Matches(
                r => r.RequestUri.ToString().Contains("/me"))))
            .Returns(Responses.Get.me_message);

        var c = ClientFactory.GetClient(fake);

        var result = await c.GetAsync(call);
        Assert.That(Responses.Get.me_content, Is.EqualTo(result));

        var meCall = Fake.GetCalls(fake).Where(call =>
            call.Method.Name == "Send" &&
            call.GetArgument<HttpRequestMessage>("request").RequestUri.ToString().Contains("/me")).First();

        var requestMessage = meCall.GetArgument<HttpRequestMessage>("request");

        var uri = requestMessage.RequestUri;
        Assert.That(called, Is.EqualTo(uri.AbsoluteUri));

        var headers = requestMessage.Headers;
        Assert.Multiple(() =>
        {
            Assert.That(Constants.APPLICATION_KEY, Is.EqualTo(headers.GetValues(Client.OVH_APP_HEADER).First()));
            Assert.That(Constants.CONSUMER_KEY, Is.EqualTo(headers.GetValues(Client.OVH_CONSUMER_HEADER).First()));
            Assert.That(currentServerTimestamp.ToString(), Is.EqualTo(headers.GetValues(Client.OVH_TIME_HEADER).First()));
            Assert.That(sig, Is.EqualTo(headers.GetValues(Client.OVH_SIGNATURE_HEADER).First()));
        });
    }

    [Test]
    public void GET_me_throws_when_timeout_expires()
    {
        var fake = A.Fake<FakeHttpMessageHandler>(a => a.CallsBaseMethods());
        MockAuthTimeCallWithFakeItEasy(fake);

        A.CallTo(() =>
            fake.Send(A<HttpRequestMessage>.That.Matches(
                r => r.RequestUri.ToString().Contains("/me"))))
            .Invokes(() => Thread.Sleep(1000))
            .Returns(Responses.Get.me_message);

        var c = ClientFactory.GetClient(fake, timeout: TimeSpan.Zero);
        Assert.ThrowsAsync<TaskCanceledException>(() => c.GetAsync("/me"));
    }

    [Test]
    public async Task GET_me_as_T()
    {
        var fake = A.Fake<FakeHttpMessageHandler>(a => a.CallsBaseMethods());
        MockAuthTimeCallWithFakeItEasy(fake);

        A.CallTo(() =>
            fake.Send(A<HttpRequestMessage>.That.Matches(
                r => r.RequestUri.ToString().Contains("/me"))))
            .Returns(Responses.Get.me_message);

        var c = ClientFactory.GetClient(fake);
        var result = await c.GetAsync<Me>("/me");

        Assert.That("Noname", Is.EqualTo(result.name));
        Assert.That("none-ovh", Is.EqualTo(result.nichandle));
        Assert.That("EUR", Is.EqualTo(result.currency.code));
        Assert.That("€", Is.EqualTo(result.currency.symbol));
    }

    [Test]
    public async Task GET_with_filter_generates_correct_signature()
    {
        var fake = A.Fake<FakeHttpMessageHandler>(a => a.CallsBaseMethods());
        MockAuthTimeCallWithFakeItEasy(fake);

        A.CallTo(() =>
            fake.Send(A<HttpRequestMessage>.That.Matches(
                r => r.RequestUri.ToString().Contains("/route"))))
            .Returns(Responses.Get.empty_message);

        var c = ClientFactory.GetClient(fake);
        var queryParams = new QueryStringParams
        {
            { "filter", "value:&é'-" },
            { "anotherfilter", "=test" }
        };
        _ = await c.GetAsync("/route", queryParams);

        var meCall = Fake.GetCalls(fake).Where(call =>
            call.Method.Name == "Send" &&
            call.GetArgument<HttpRequestMessage>("request").RequestUri.ToString().Contains("/route")).First();

        var requestMessage = meCall.GetArgument<HttpRequestMessage>("request");
        var headers = requestMessage.Headers;
        Assert.That("$1$098b93d342b6db4848ec448063be2b6884e94723", Is.EqualTo(headers.GetValues(Client.OVH_SIGNATURE_HEADER).First()));
    }
}