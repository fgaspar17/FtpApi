using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace FtpApi.Data.Models;

public class ApiUser : IdentityUser
{
    public IEnumerable<FileMetadata> FileMetadatas { get; set; } = new List<FileMetadata>();
    public ApiUser() { }
}
