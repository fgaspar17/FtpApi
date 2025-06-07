namespace FtpApi.Application.Exceptions;

public class UserRegistrationException : Exception
{
    public IEnumerable<string> Errors { get; }

    public UserRegistrationException(string message, IEnumerable<string> errors = null)
        : base(message)
    {
        Errors = errors ?? Enumerable.Empty<string>();
    }
}