using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FtpApi.Application.DTOs;

public class UserRegisterDto
{
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? ConfirmedPassword { get; set; }
}
