using System;
using VillaAPI.Models;
using VillaAPI.Models.DTO;

namespace VillaAPI.Repository.IRepository;

public interface IUserRepository
{
    bool IsUniqueUser(string username);
    Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
    Task<UserDTO> Register(RegisterationRequestDTO registerationRequestDTO);
}

