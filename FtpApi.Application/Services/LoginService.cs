using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FtpApi.Application.DTOs;

namespace FtpApi.Application.Services
{
    public class LoginService
    {
        public static async Task Login(UserLoginDto loginDto, IValidator<UserLoginDto> validator)
        {
            validator.ValidateAndThrow(loginDto);

            // TODO: Mappear DTO
        }
    }
}
