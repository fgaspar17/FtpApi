using FtpApi.Application.DTOs;
using FtpApi.Data.Models;

namespace FtpApi.Application.Mappers;

internal class RegisterMapper
{
    public static ApiUser Map(UserRegisterDto dto)
    {
        return new ApiUser
        {
            UserName = dto.UserName,
        };
    }
}