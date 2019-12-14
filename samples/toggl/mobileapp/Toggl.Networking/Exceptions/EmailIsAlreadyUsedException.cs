namespace Toggl.Networking.Exceptions
{
    public sealed class EmailIsAlreadyUsedException : ClientErrorException
    {
        public EmailIsAlreadyUsedException(BadRequestException badRequestException)
            : base(badRequestException.Request, badRequestException.Response, badRequestException.Message)
        {
        }
    }
}
