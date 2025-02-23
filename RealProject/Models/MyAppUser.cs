using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RealProject.Models
{
    public class MyAppUser : IdentityUser
    {

        public string? Contact { get; set; }

        public string? Address { get; set; }


        public string? FullName { get; set; }
        public string? Role { get; set; }
    }
}
