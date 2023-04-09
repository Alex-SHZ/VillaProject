using System;
namespace VillaAPI.Models.DTO;

public class LoginResponseDTO
{
    public UserDTO User { get; set; }
    public string Token { get; set; }
}

