using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentValidation;
using FtpApi.Application.DTOs;
using FtpApi.Application.Exceptions;
using FtpApi.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace FtpApi.Application.Services
{
    public class LoginService
    {
        private readonly IValidator<UserLoginDto> _validator;
        private readonly UserManager<ApiUser> _userManager;
        private readonly ILogger<LoginService> _logger;

        public LoginService(
            IValidator<UserLoginDto> validator,
            UserManager<ApiUser> userManager,
            ILogger<LoginService> logger)
        {
            _validator = validator;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<string> Login(UserLoginDto loginDto, IConfiguration config)
        {
            _validator.ValidateAndThrow(loginDto);

            var user = await _userManager.FindByNameAsync(loginDto.UserName);

            if (user is null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
                throw new CustomValidationException("Invalid login attemp");

            user = await _userManager.FindByNameAsync(loginDto.UserName);

            List<Claim> claims = new();
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(config["JWT:SigningKey"])),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: config["JWT:Issuer"],
                audience: config["JWT:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddSeconds(300),
                signingCredentials: signingCredentials
                );

            var jwtString = new JwtSecurityTokenHandler().WriteToken(token);

            return jwtString;
        }
    }
}
