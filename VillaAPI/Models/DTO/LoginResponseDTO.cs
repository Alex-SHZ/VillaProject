using System;
namespace VillaAPI.Models.DTO;

public class LoginResponseDTO
{
    public LocalUser User { get; set; }
    public string Token { get; set; }
}

