using FakeItEasy;

using NUnit.Framework;

using Ovh.Api;
using Ovh.Api.Testing;

using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ovh.Test;

[TestFixture]
public class DeleteRequests
{
    private static long currentClientTimestamp = 1566485765;
    private static long currentServerTimestamp = 1566485767;
    private static DateTimeOffset currentDateTime = DateTimeOffset.FromUnixTimeSeconds(currentClientTimestamp);
    private static ITimeProvider timeProvider = A.Fake<ITimeProvider>();

    public DeleteRequests()
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
    public async Task DELETE_as_string()
    {
        var fake = A.Fake<FakeHttpMessageHandler>(a => a.CallsBaseMethods());
        MockAuthTimeCallWithFakeItEasy(fake);

        A.CallTo(() =>
            fake.Send(A<HttpRequestMessage>.That.Matches(
                r => r.RequestUri.ToString().Contains("/ip/127.0.0.1"))))
            .Returns(Responses.Delete.nullAsHttpResponseMessage);

        var c = ClientFactory.GetClient(fake);
        var result = await c.DeleteAsync("/ip/127.0.0.1");
        Assert.That(Responses.Delete.nullAsJsonString, Is.EqualTo(result));

        var meCall = Fake.GetCalls(fake).Where(call =>
            call.Method.Name == "Send" &&
            call.GetArgument<HttpRequestMessage>("request").RequestUri.ToString().Contains("/ip/127.0.0.1")).First();

        var requestMessage = meCall.GetArgument<HttpRequestMessage>("request");
        var headers = requestMessage.Headers;
        Assert.Multiple(() =>
        {
            Assert.That(Constants.APPLICATION_KEY, Is.EqualTo(headers.GetValues(Client.OVH_APP_HEADER).First()));
            Assert.That(Constants.CONSUMER_KEY, Is.EqualTo(headers.GetValues(Client.OVH_CONSUMER_HEADER).First()));
            Assert.That(currentServerTimestamp.ToString(), Is.EqualTo(headers.GetValues(Client.OVH_TIME_HEADER).First()));
            Assert.That("$1$610ebc657a19d6b444264f998291a4f24bc3227d", Is.EqualTo(headers.GetValues(Client.OVH_SIGNATURE_HEADER).First()));
        });
    }

    [Test]
    public async Task DELETE_as_T()
    {
        var fake = A.Fake<FakeHttpMessageHandler>(a => a.CallsBaseMethods());
        MockAuthTimeCallWithFakeItEasy(fake);

        A.CallTo(() =>
            fake.Send(A<HttpRequestMessage>.That.Matches(
                r => r.RequestUri.ToString().Contains("/ip/127.0.0.1"))))
            .Returns(Responses.Get.empty_message);

        var c = ClientFactory.GetClient(fake);
        var queryParams = new QueryStringParams
        {
            { "filter", "value:&é'-" },
            { "anotherfilter", "=test" }
        };
        var result = await c.DeleteAsync<object>("/ip/127.0.0.1");
        Assert.That(result, Is.EqualTo(null));
    }
}