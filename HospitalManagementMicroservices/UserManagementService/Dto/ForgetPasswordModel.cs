﻿using System.ComponentModel.DataAnnotations;

namespace UserManagementService.Dto
{
    public class ForgetPasswordModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
    }
}
