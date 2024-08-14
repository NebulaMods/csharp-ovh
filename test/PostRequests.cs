using FakeItEasy;

using Newtonsoft.Json;

using NUnit.Framework;

using Ovh.Api;
using Ovh.Api.Testing;
using Ovh.Test.Models;

using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ovh.Test;

[TestFixture]
public class PostRequests
{
    private static long currentClientTimestamp = 1566485765;
    private static long currentServerTimestamp = 1566485767;
    private static DateTimeOffset currentDateTime = DateTimeOffset.FromUnixTimeSeconds(currentClientTimestamp);
    private static ITimeProvider timeProvider = A.Fake<ITimeProvider>();

    public PostRequests()
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
    public async Task POST_with_no_data_and_result_as_string()
    {
        var testHandler = A.Fake<FakeHttpMessageHandler>(a => a.CallsBaseMethods());
        MockAuthTimeCallWithFakeItEasy(testHandler);
        A.CallTo(() =>
            testHandler.Send(A<HttpRequestMessage>.That.Matches(
                r => r.RequestUri.ToString().Contains("/me/geolocation"))))
            .Returns(Responses.Post.me_geolocation_message);

        var c = ClientFactory.GetClient(testHandler).AsTestable(timeProvider);
        var result = await c.PostAsync("/me/geolocation", null);
        Assert.That(Responses.Post.me_geolocation_content, Is.EqualTo(result));

        var geolocCall = Fake.GetCalls(testHandler).Where(call =>
            call.Method.Name == "Send" &&
            call.GetArgument<HttpRequestMessage>("request").RequestUri.ToString().Contains("/me/geolocation")).First();

        var requestMessage = geolocCall.GetArgument<HttpRequestMessage>("request");
        var headers = requestMessage.Headers;
        Assert.Multiple(() =>
        {
            Assert.That(Constants.APPLICATION_KEY, Is.EqualTo(headers.GetValues(Client.OVH_APP_HEADER).First()));
            Assert.That(Constants.CONSUMER_KEY, Is.EqualTo(headers.GetValues(Client.OVH_CONSUMER_HEADER).First()));
            Assert.That(currentServerTimestamp.ToString(), Is.EqualTo(headers.GetValues(Client.OVH_TIME_HEADER).First()));
            Assert.That("$1$3473ad8790d09e6d28f8a9d6f09a05c1f5f0bbfc", Is.EqualTo(headers.GetValues(Client.OVH_SIGNATURE_HEADER).First()));
        });
    }

    [Test]
    public async Task POST_with_no_data_and_result_as_T()
    {
        var testHandler = A.Fake<FakeHttpMessageHandler>(a => a.CallsBaseMethods());
        MockAuthTimeCallWithFakeItEasy(testHandler);
        A.CallTo(() =>
            testHandler.Send(A<HttpRequestMessage>.That.Matches(
                r => r.RequestUri.ToString().Contains("/me/geolocation"))))
            .Returns(Responses.Post.me_geolocation_message);

        var c = ClientFactory.GetClient(testHandler).AsTestable(timeProvider);
        var result = await c.PostAsync<Geolocation>("/me/geolocation");
        Assert.That("eo", Is.EqualTo(result.countryCode));
        Assert.That("256.0.0.1", Is.EqualTo(result.ip));
        Assert.That("Atlantis", Is.EqualTo(result.continent));
    }

    [Test]
    public async Task POST_with_raw_string_data_and_string_result()
    {
        var testHandler = A.Fake<FakeHttpMessageHandler>(a => a.CallsBaseMethods());
        MockAuthTimeCallWithFakeItEasy(testHandler);
        A.CallTo(() =>
            testHandler.Send(A<HttpRequestMessage>.That.Matches(
                r => r.RequestUri.ToString().Contains("/me/contact"))))
            .Returns(Responses.Post.me_contact_message);

        var c = ClientFactory.GetClient(testHandler).AsTestable(timeProvider);
        var result = await c.PostStringAsync("/me/contact", "Fake content");
        Assert.That(Responses.Post.me_contact_content, Is.EqualTo(result));

        var contactCall = Fake.GetCalls(testHandler).Where(call =>
            call.Method.Name == "Send" &&
            call.GetArgument<HttpRequestMessage>("request").RequestUri.ToString().Contains("/me/contact")).First();

        var requestMessage = contactCall.GetArgument<HttpRequestMessage>("request");
        var headers = requestMessage.Headers;
        Assert.Multiple(() =>
        {
            Assert.That(Constants.APPLICATION_KEY, Is.EqualTo(headers.GetValues(Client.OVH_APP_HEADER).First()));
            Assert.That(Constants.CONSUMER_KEY, Is.EqualTo(headers.GetValues(Client.OVH_CONSUMER_HEADER).First()));
            Assert.That(currentServerTimestamp.ToString(), Is.EqualTo(headers.GetValues(Client.OVH_TIME_HEADER).First()));
            Assert.That("$1$19a8f2db1a3b2b89b231c7872332b6ba117d8bd7", Is.EqualTo(headers.GetValues(Client.OVH_SIGNATURE_HEADER).First()));
        });
    }

    [Test]
    public async Task POST_with_string_to_be_serialized_data_and_T_result()
    {
        var testHandler = A.Fake<FakeHttpMessageHandler>(a => a.CallsBaseMethods());
        MockAuthTimeCallWithFakeItEasy(testHandler);
        A.CallTo(() =>
            testHandler.Send(A<HttpRequestMessage>.That.Matches(
                r => r.RequestUri.ToString().Contains("/me/contact"))))
            .Returns(Responses.Post.me_contact_message);

        var c = ClientFactory.GetClient(testHandler).AsTestable(timeProvider);
        var result = await c.PostAsync<Contact>("/me/contact", "Fake content");
        Assert.That("deleteme@deleteme.deleteme", Is.EqualTo(result.email));

        var contactCall = Fake.GetCalls(testHandler).Where(call =>
            call.Method.Name == "Send" &&
            call.GetArgument<HttpRequestMessage>("request").RequestUri.ToString().Contains("/me/contact")).First();

        var requestMessage = contactCall.GetArgument<HttpRequestMessage>("request");
        var headers = requestMessage.Headers;
        Assert.Multiple(() =>
        {
            Assert.That(Constants.APPLICATION_KEY, Is.EqualTo(headers.GetValues(Client.OVH_APP_HEADER).First()));
            Assert.That(Constants.CONSUMER_KEY, Is.EqualTo(headers.GetValues(Client.OVH_CONSUMER_HEADER).First()));
            Assert.That(currentServerTimestamp.ToString(), Is.EqualTo(headers.GetValues(Client.OVH_TIME_HEADER).First()));
            Assert.That("$1$8a6f2668c14048c59ca957bc26b16a29180ffb03", Is.EqualTo(headers.GetValues(Client.OVH_SIGNATURE_HEADER).First()));
        });
    }

    [Test]
    public async Task POST_with_string_to_be_serialized_data_and_string_result()
    {
        var testHandler = A.Fake<FakeHttpMessageHandler>(a => a.CallsBaseMethods());
        MockAuthTimeCallWithFakeItEasy(testHandler);
        A.CallTo(() =>
            testHandler.Send(A<HttpRequestMessage>.That.Matches(
                r => r.RequestUri.ToString().Contains("/me/contact"))))
            .Returns(Responses.Post.me_contact_message);

        var c = ClientFactory.GetClient(testHandler).AsTestable(timeProvider);
        var result = await c.PostAsync("/me/contact", "Fake content");
        Assert.That(Responses.Post.me_contact_content, Is.EqualTo(result));

        var contactCall = Fake.GetCalls(testHandler).Where(call =>
            call.Method.Name == "Send" &&
            call.GetArgument<HttpRequestMessage>("request").RequestUri.ToString().Contains("/me/contact")).First();

        var requestMessage = contactCall.GetArgument<HttpRequestMessage>("request");
        var headers = requestMessage.Headers;
        Assert.Multiple(() =>
        {
            Assert.That(Constants.APPLICATION_KEY, Is.EqualTo(headers.GetValues(Client.OVH_APP_HEADER).First()));
            Assert.That(Constants.CONSUMER_KEY, Is.EqualTo(headers.GetValues(Client.OVH_CONSUMER_HEADER).First()));
            Assert.That(currentServerTimestamp.ToString(), Is.EqualTo(headers.GetValues(Client.OVH_TIME_HEADER).First()));
            Assert.That("$1$8a6f2668c14048c59ca957bc26b16a29180ffb03", Is.EqualTo(headers.GetValues(Client.OVH_SIGNATURE_HEADER).First()));
        });
    }

    [Test]
    public async Task POST_with_T_data_and_string_result()
    {
        var testHandler = A.Fake<FakeHttpMessageHandler>(a => a.CallsBaseMethods());
        MockAuthTimeCallWithFakeItEasy(testHandler);
        var dummyContact = new Contact
        {
            address = new Address
            {
                city = "deleteme",
                country = "FR",
                line1 = "deleteme",
                zip = "00000"
            },
            email = "deleteme@deleteme.deleteme",
            firstName = "deleteme",
            language = "fr_FR",
            lastName = "deleteme",
            legalForm = "individual",
            phone = "0000000000"
        };
        A.CallTo(() =>
            testHandler.Send(A<HttpRequestMessage>.That.Matches(
                r => r.RequestUri.ToString().Contains("/me/contact"))))
            .Returns(Responses.Post.me_contact_message);

        var c = ClientFactory.GetClient(testHandler).AsTestable(timeProvider);
        var result = await c.PostAsync<Contact>("/me/contact", dummyContact);

        //Ensure that the call went through correctly
        Assert.That(123456, Is.EqualTo(result.id));

        var contactCall = Fake.GetCalls(testHandler).Where(call =>
            call.Method.Name == "Send" &&
            call.GetArgument<HttpRequestMessage>("request").RequestUri.ToString().Contains("/me/contact")).First();

        var requestMessage = contactCall.GetArgument<HttpRequestMessage>("request");

        // Ensure that we sent a serialized version of the dummy contact
        var sendtObject = JsonConvert.DeserializeObject<Contact>(await requestMessage.Content.ReadAsStringAsync());
        Assert.That(dummyContact.address.country, Is.EqualTo(sendtObject.address.country));
        Assert.That(dummyContact.address.zip, Is.EqualTo(sendtObject.address.zip));
        Assert.That(dummyContact.email, Is.EqualTo(sendtObject.email));
    }
}