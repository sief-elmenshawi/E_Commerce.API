using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace E_Commerce.Application.DTOs.Authentications
{
    public class AddressDto
    {
        [Required]
        public string City { get; set; } = default!;
        [Required]
        public string Street { get; set; } = default!;
        [Required]
        public string Country { get; set; } = default!;
        [Required]
        public string FirstName { get; set; } = default!;
        [Required]
        public string LastName { get; set; } = default!;
    }
}
