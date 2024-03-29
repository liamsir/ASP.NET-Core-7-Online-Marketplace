﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace MVCWebAppIsmane.Models
{
            public class User : IdentityUser        
            {

                
                [Required(ErrorMessage = "Name is required")]
                public string Name { get; set; }
                
                public string? City { get; set; }


                [ValidateNever]
                public List<Achat> Achats { get; set; }
            }
}












 /*
                public string UserRole { get; set; } = "Customer"; 
            
                [EmailAddress(ErrorMessage = "Invalid email address")]
                public string Email { get; set; }

                [Required(ErrorMessage = "Password is required")]
                [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
                public string Password { get; set; }

        
                [Phone(ErrorMessage = "Invalid phone number")]            
                public string PhoneNumber { get; set; }
*/