using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Toggl.Networking.Exceptions;
using Toggl.Networking.Helpers;
using Toggl.Networking.Network;
using Xunit;
using static System.Net.HttpStatusCode;
using NotImplementedException = Toggl.Networking.Exceptions.NotImplementedException;

namespace Toggl.Networking.Tests.Exceptions
{
    public sealed class ApiErrorResponsesTests
    {
        public sealed class ClientErrors
        {
            [Theory, LogIfTooSlow]
            [MemberData(nameof(ClientErrorsList), MemberType = typeof(ApiErrorResponsesTests))]
            public void ReturnsClientErrorException(HttpStatusCode httpStatusCode, Type expectedExceptionType)
            {
                var request = createRequest(HttpMethod.Get);
                var response = createErrorResponse(httpStatusCode);

                var exception = ApiExceptions.For(request, response);

                exception.Should().BeAssignableTo<ClientErrorException>().And.BeOfType(expectedExceptionType);
            }
        }

        public sealed class ServerErrors
        {
            [Theory, LogIfTooSlow]
            [MemberData(nameof(ServerErrorsList), MemberType = typeof(ApiErrorResponsesTests))]
            public void ReturnsServerErrorException(HttpStatusCode httpStatusCode, Type expectedExceptionType)
            {
                var request = createRequest(HttpMethod.Get);
                var response = createErrorResponse(httpStatusCode);

                var exception = ApiExceptions.For(request, response);

                exception.Should().BeAssignableTo<ServerErrorException>().And.BeOfType(expectedExceptionType);
            }
        }

        public sealed class UnknownErrors
        {
            [Theory, LogIfTooSlow]
            [MemberData(nameof(UnknownErrorsList), MemberType = typeof(ApiErrorResponsesTests))]
            public void ReturnsUnknownApiError(HttpStatusCode httpStatusCode)
            {
                var request = createRequest(HttpMethod.Get);
                var response = createErrorResponse(httpStatusCode);

                var exception = ApiExceptions.For(request, response);

                exception.Should().BeOfType<UnknownApiErrorException>()
                    .Which.HttpCode.Should().Be(httpStatusCode);
            }
        }

        public sealed class Serialization
        {
            [Theory, LogIfTooSlow]
            [MemberData(nameof(ClientErrorsList), MemberType = typeof(ApiErrorResponsesTests))]
            [MemberData(nameof(ServerErrorsList), MemberType = typeof(ApiErrorResponsesTests))]
            public void CreatesAStringStartingWithExceptionName(HttpStatusCode statusCode, Type exceptionType)
            {
                var request = createRequest(HttpMethod.Get);
                var response = createErrorResponse(statusCode);
                var exception = ApiExceptions.For(request, response);

                var serialized = exception.ToString();

                serialized.Should().StartWith($"{exceptionType.Name} ");
            }

            [Fact, LogIfTooSlow]
            public void CreatesAStringWithBodyAndNoHeaders()
            {
                string body = "Body.";
                var endpoint = new Uri("https://www.some.url");
                var method = new HttpMethod("GET");
                var request = new Request("", endpoint, new HttpHeader[0], method);
                var response = new Response(body, false, "plain/text", new List<KeyValuePair<string, IEnumerable<string>>>(), HttpStatusCode.InternalServerError);
                var exception = new InternalServerErrorException(request, response, "Custom message.");
                var expectedSerialization = $"InternalServerErrorException (Custom message.) for request {method} {endpoint} with response {{\"status\":\"500 InternalServerError\",\"headers\":{{}},\"body\":\"{body}\"}}";

                var serialized = exception.ToString();

                serialized.Should().Be(expectedSerialization);
            }

            [Fact, LogIfTooSlow]
            public void CreatesAStringWithBodyAndWithHeaders()
            {
                string body = "Body of a response with headers.";
                var endpoint = new Uri("https://www.some.url/endpoint");
                var method = new HttpMethod("GET");
                var request = new Request("", endpoint, new HttpHeader[0], method);
                var headers = new[] { new KeyValuePair<string, IEnumerable<string>>("abc", new[] { "a", "b", "c" }) };
                var response = new Response(body, false, "plain/text", headers, HttpStatusCode.InternalServerError);
                var exception = new InternalServerErrorException(request, response, "Custom message.");
                var expectedSerialization = $"InternalServerErrorException (Custom message.) for request {method} {endpoint} with response {{\"status\":\"500 InternalServerError\",\"headers\":{{\"abc\":[\"a\",\"b\",\"c\"]}},\"body\":\"{body}\"}}";

                var serialized = exception.ToString();

                serialized.Should().Be(expectedSerialization);
            }

            [Fact, LogIfTooSlow]
            public void DeserializesLocalizedErrorMessageAsJsonInResponse()
            {
                var defaultMessage = "Default message";
                var message = "Couldn't find workspace with id blah blah blah...";
                var body = $"{{\"error_message\": {{\"id\": \"machine-readable-id\", \"default_message\": \"{message}\", \"values\": {{\"number1\": \"2\", \"number2\": \"5\" }}}}}}";
                var endpoint = new Uri("https://www.some.url");
                var method = new HttpMethod("GET");
                var request = new Request("", endpoint, new HttpHeader[0], method);
                var response = new Response(body, false, "application/json", new List<KeyValuePair<string, IEnumerable<string>>>(), HttpStatusCode.NotFound);
                var exception = new ApiException(request, response, defaultMessage);

                exception.LocalizedApiErrorMessage.Should().Be(message);
            }

            [Theory, LogIfTooSlow]
            [InlineData("null")]
            [InlineData("{}")]
            [InlineData("{\"error_message\":null}")]
            [InlineData("{\"message\":\"this is the oldest format\"}")]
            [InlineData("{\"error_message\": \"this is the old format now\"}")]
            [InlineData("{\"notAMessage\":\"hi\"}")]
            public void ReturnsFallbackLocalisedErrorMessageIfJsonErrorHasNoDefaultMessage(string noMessageJson)
            {
                var defaultMessage = "Default message";
                var endpoint = new Uri("https://www.some.url");
                var method = new HttpMethod("GET");
                var request = new Request("", endpoint, new HttpHeader[0], method);
                var response = new Response(noMessageJson, false, "application/json", new List<KeyValuePair<string, IEnumerable<string>>>(), HttpStatusCode.NotFound);
                var exception = new ApiException(request, response, defaultMessage);

                exception.LocalizedApiErrorMessage.Should().Be("Encountered unexpected error.");
            }

            [Theory, LogIfTooSlow]
            [InlineData("{")]
            [InlineData("}")]
            [InlineData("{\"error_message\":}")]
            [InlineData("\"\"")]
            [InlineData("This is an error.")]
            public void ReturnsFallbackLocalisedErrorMessageIfJsonErrorHasInvalidSyntax(string brokenJson)
            {
                var defaultMessage = "Default message";
                var endpoint = new Uri("https://www.some.url");
                var method = new HttpMethod("GET");
                var request = new Request("", endpoint, new HttpHeader[0], method);
                var response = new Response(brokenJson, false, "application/json", new List<KeyValuePair<string, IEnumerable<string>>>(), HttpStatusCode.NotFound);
                var exception = new ApiException(request, response, defaultMessage);

                exception.LocalizedApiErrorMessage.Should().Be("Encountered unexpected error.");
            }

            [Fact, LogIfTooSlow]
            public void DeserializesLocalizedErrorMessageAsTextInResponse()
            {
                var defaultMessage = "Default message";
                var body = "Couldn't find workspace with id blah blah blah....";
                var endpoint = new Uri("https://www.some.url");
                var method = new HttpMethod("GET");
                var request = new Request("", endpoint, new HttpHeader[0], method);
                var response = new Response(body, false, "text/plain", new List<KeyValuePair<string, IEnumerable<string>>>(), HttpStatusCode.NotFound);
                var exception = new ApiException(request, response, defaultMessage);

                exception.LocalizedApiErrorMessage.Should().Be(body);
            }

            [Fact, LogIfTooSlow]
            public void DeserializesLocalizedErrorMessageAsTextInResponseForAnyContentType()
            {
                var defaultMessage = "Default message";
                var body = "Couldn't find workspace with id blah blah blah....";
                var endpoint = new Uri("https://www.some.url");
                var method = new HttpMethod("GET");
                var request = new Request("", endpoint, new HttpHeader[0], method);
                var response = new Response(body, false, "foo/bar", new List<KeyValuePair<string, IEnumerable<string>>>(), HttpStatusCode.NotFound);
                var exception = new ApiException(request, response, defaultMessage);

                exception.LocalizedApiErrorMessage.Should().Be(body);
            }
        }

        private static Request createRequest(HttpMethod method)
            => new Request("{\"a\":123}", new Uri("https://integration.tests"), new[] { new HttpHeader("X", "Y") }, method);

        private static Response createErrorResponse(HttpStatusCode code, string contentType = "plain/text", string rawData = "")
            => new Response(rawData, false, contentType, new List<KeyValuePair<string, IEnumerable<string>>>(), code);

        public static IEnumerable<object[]> ClientErrorsList
            => new[]
            {
                new object[] { BadRequest, typeof(BadRequestException) },
                new object[] { Unauthorized, typeof(UnauthorizedException) },
                new object[] { PaymentRequired, typeof(PaymentRequiredException) },
                new object[] { Forbidden, typeof(ForbiddenException) },
                new object[] { NotFound, typeof(NotFoundException) },
                new object[] { Gone, typeof(ApiDeprecatedException) },
                new object[] { RequestEntityTooLarge, typeof(RequestEntityTooLargeException) },
                new object[] { 418, typeof(ClientDeprecatedException) }, // HTTP 418 - I Am a Teapot
                new object[] { 429, typeof(TooManyRequestsException) } // HTTP 429 - Too Many Requests
            };

        public static IEnumerable<object[]> ServerErrorsList
            => new[]
            {
                new object[] { InternalServerError, typeof(InternalServerErrorException) },
                new object[] { NotImplemented, typeof(NotImplementedException) },
                new object[] { BadGateway, typeof(BadGatewayException) },
                new object[] { ServiceUnavailable, typeof(ServiceUnavailableException) },
                new object[] { GatewayTimeout, typeof(GatewayTimeoutException) },
                new object[] { HttpVersionNotSupported, typeof(HttpVersionNotSupportedException) }
            };

        private static IEnumerable<object[]> KnownErrorsList
            => ClientErrorsList.Concat(ServerErrorsList);

        private static bool IsKnownError(int code)
            => KnownErrorsList.Any(item => (int)item[0] == code);

        public static IEnumerable<object[]> UnknownErrorsList
            => Enumerable.Range(400, 200)
                .Where(code => !IsKnownError(code))
                .Select(code => new object[] { (HttpStatusCode)code });
    }
}
