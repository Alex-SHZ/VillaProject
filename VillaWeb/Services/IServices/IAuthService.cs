using System;
using VillaWeb.Models.DTO;

namespace VillaWeb.Services.IServices;

public interface IAuthService
{
    Task<T> LoginAsync<T>(LoginRequestDTO objToCreate);
    Task<T> RegisterAsync<T>(RegisterationRequestDTO objToCreate);
}

