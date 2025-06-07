using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FtpApi.Application.Exceptions;

[Serializable]
public class CustomValidationException : Exception
{
    public IEnumerable<string> Errors { get; }
    public CustomValidationException(IEnumerable<string> errors = null) : base() 
    {
        Errors = errors ?? [];
    }
    public CustomValidationException(string message, IEnumerable<string> errors = null) : base(message) 
    {
        Errors = errors ?? [];
    }

    public CustomValidationException(string message, Exception inner, IEnumerable<string> errors = null) : base(message, inner) 
    {
        Errors = errors ?? [];
    }
    public CustomValidationException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context,
      IEnumerable<string> errors = null) : base(info, context) 
    {
        Errors = errors ?? [];
    }

}