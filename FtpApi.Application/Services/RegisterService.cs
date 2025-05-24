using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FtpApi.Application.DTOs;
using FtpApi.Application.Mappers;
using FtpApi.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FtpApi.Application.Services;

public class RegisterService
{
    private readonly IValidator<UserRegisterDto> _validator;
    private readonly UserManager<ApiUser> _userManager;
    private readonly ILogger<RegisterService> _logger;

    public RegisterService(
        IValidator<UserRegisterDto> validator,
        UserManager<ApiUser> userManager,
        ILogger<RegisterService> logger)
    {
        _validator = validator;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<ApiUser> Register(UserRegisterDto registerDto)
    {
        _logger.LogInformation("User registration attempt for: {UserName}", registerDto.UserName);

        _validator.ValidateAndThrow(registerDto);

        var user = RegisterMapper.Map(registerDto);
        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            _logger.LogError("Failed to register user {UserName}. Errors: {Errors}", registerDto.UserName, errors);
            throw new InvalidOperationException($"User registration failed: {errors}");
        }

        _logger.LogInformation("User successfully registered: {UserName}", registerDto.UserName);

        return user;
    }
}
