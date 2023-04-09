using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using VillaAPI.Models;
using VillaAPI.Models.DTO;
using VillaAPI.Repository.IRepository;

namespace VillaAPI.Controllers;

[Route("api/v{version:apiVersion}/UsersAuth")]
[ApiController]
[ApiVersionNeutral]
public class UsersController : Controller
{
    private readonly IUserRepository _userRepo;
    protected APIResponse _response;
    public UsersController(IUserRepository userRepo)
    {
        _userRepo = userRepo;
        _response = new();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
    {
        LoginResponseDTO loginResponse = await _userRepo.Login(model);
        if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
        {
            _response.StatucCode = HttpStatusCode.BadRequest;
            _response.IsSucces = false;
            _response.ErrorMessages.Add("Username or password is incorrect");
            return BadRequest(_response);
        }
        _response.StatucCode = HttpStatusCode.OK;
        _response.IsSucces = true;
        _response.Result = loginResponse;
        return Ok(_response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterationRequestDTO model)
    {
        bool ifUserNameUnique = _userRepo.IsUniqueUser(model.UserName);
        if (!ifUserNameUnique)
        {
            _response.StatucCode = HttpStatusCode.BadRequest;
            _response.IsSucces = false;
            _response.ErrorMessages.Add("Username already exists");
            return BadRequest(_response);
        }

        UserDTO user = await _userRepo.Register(model);
        if (user == null)
        {
            _response.StatucCode = HttpStatusCode.BadRequest;
            _response.IsSucces = false;
            _response.ErrorMessages.Add("Error while registering");
            return BadRequest(_response);
        }
        _response.StatucCode = HttpStatusCode.OK;
        _response.IsSucces = true;
        return Ok(_response);
    }
}

