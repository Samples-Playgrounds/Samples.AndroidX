using Toggl.Networking.Exceptions;
using Toggl.Networking.Network;

namespace Toggl.Networking.Helpers
{
    internal static class ApiExceptions
    {
        public static ApiException For(IRequest request, IResponse response)
        {
            var rawData = response.RawData;
            switch (response.StatusCode)
            {
                // Client errors
                case ApiDeprecatedException.CorrespondingHttpCode:
                    return new ApiDeprecatedException(request, response);
                case BadRequestException.CorrespondingHttpCode:
                    return new BadRequestException(request, response);
                case ClientDeprecatedException.CorrespondingHttpCode:
                    return new ClientDeprecatedException(request, response);
                case ForbiddenException.CorrespondingHttpCode:
                    return new ForbiddenException(request, response);
                case NotFoundException.CorrespondingHttpCode:
                    return new NotFoundException(request, response);
                case PaymentRequiredException.CorrespondingHttpCode:
                    return new PaymentRequiredException(request, response);
                case RequestEntityTooLargeException.CorrespondingHttpCode:
                    return new RequestEntityTooLargeException(request, response);
                case TooManyRequestsException.CorrespondingHttpCode:
                    return new TooManyRequestsException(request, response);
                case UnauthorizedException.CorrespondingHttpCode:
                    return new UnauthorizedException(request, response);

                // Server errors
                case InternalServerErrorException.CorrespondingHttpCode:
                    return new InternalServerErrorException(request, response);
                case NotImplementedException.CorrespondingHttpCode:
                    return new NotImplementedException(request, response);
                case BadGatewayException.CorrespondingHttpCode:
                    return new BadGatewayException(request, response);
                case ServiceUnavailableException.CorrespondingHttpCode:
                    return new ServiceUnavailableException(request, response);
                case GatewayTimeoutException.CorrespondingHttpCode:
                    return new GatewayTimeoutException(request, response);
                case HttpVersionNotSupportedException.CorrespondingHttpCode:
                    return new HttpVersionNotSupportedException(request, response);

                default:
                    return new UnknownApiErrorException(request, response);
            }
        }
    }
}
