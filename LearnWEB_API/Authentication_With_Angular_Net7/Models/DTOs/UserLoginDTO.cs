﻿using System.ComponentModel.DataAnnotations;

namespace Authentication_With_Angular_Net7.Models.DTOs;

public class UserLoginDTO
{
    [Required]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}
