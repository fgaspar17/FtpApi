using System.Security.Claims;
using FtpApi.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace FtpApi.Users;

public static class UserHelper
{
    public static async Task<string> GetUserId(this HttpContext context, UserManager<ApiUser> userManager)
    {
        var username = context.User.Claims.Where(claim => claim.Type == ClaimTypes.Name).FirstOrDefault().Value;
        var user = await userManager.FindByNameAsync(username);
        return user.Id;
    }
}
