using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace FtpApi.Startup;

public static class AuthenticationConfig
{
    public static void AddAuthenticationServices(this IServiceCollection services, ConfigurationManager config)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = config["JWT:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = config["JWT:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(config["JWT:SigningKey"])),
                    
                };
            });
    }
}
