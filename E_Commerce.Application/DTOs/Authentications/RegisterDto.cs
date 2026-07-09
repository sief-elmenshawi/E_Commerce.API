using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace E_Commerce.Application.DTOs.Authentications
{
    public class RegisterDto
    {
        [Required,EmailAddress]
        public string Email { get; set; } = default!;
        [Required]
        public string Password { get; set; } = default!;
        [Required]
        public string UserName { get; set; } = default!;
        [Required]
        public string DisplayName { get; set; } = default!;
        public string? PhoneNumber { get; set; } 
    }
}
