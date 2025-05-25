using FtpApi.Application.DTOs;
using FtpApi.Data.Models;

namespace FtpApi.Application.Mappers;

internal class LoginMapper
{
    public static ApiUser Map(UserLoginDto loginDto)
    {
        return new ApiUser
        {
            UserName = loginDto.UserName,
        };
    }
}