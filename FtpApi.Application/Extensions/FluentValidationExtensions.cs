using FluentValidation;

namespace FtpApi.Application.Extensions;

public static class FluentValidationExtensions
{
    public static async Task ValidateAndThrowCustomAsync<T>(this IValidator<T> validator, T data)
    {
        try
        {
            await validator.ValidateAndThrowAsync(data);
        }
        catch (ValidationException ex)
        {
            var errors = string.Join("; ", ex.Errors.Select(e => e.ErrorMessage));
            throw new Exceptions.CustomValidationException($"Validation errors: {errors}", ex);
        }
    }
}
