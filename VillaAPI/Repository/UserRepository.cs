using System;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VillaAPI.Data;
using VillaAPI.Models;
using VillaAPI.Models.DTO;
using Microsoft.AspNetCore.Identity;
using AutoMapper;

namespace VillaAPI.Repository.IRepository;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private string secretKey { get; set; }
    private readonly IMapper _mapper;

    public UserRepository(ApplicationDbContext db,
        IConfiguration configuration,
        UserManager<ApplicationUser> userManager,
        IMapper mapper,
        RoleManager<IdentityRole> roleManager)
    {
        _db = db;
        secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        _userManager = userManager;
        _mapper = mapper;
        _roleManager = roleManager;
    }

    public bool IsUniqueUser(string username)
    {
        ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(x => x.UserName == username);
        if (user == null) return true;
        return false;
    }

    public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
    {
        ApplicationUser user = _db.ApplicationUsers
            .FirstOrDefault(u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower());

        bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

        if (user == null || isValid == false)
        {
            return new LoginResponseDTO()
            {
                Token = "",
                User = null
            };
        } 

        IList<string> roles = await _userManager.GetRolesAsync(user);

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        byte[] key = Encoding.ASCII.GetBytes(secretKey);

        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.UserName.ToString()),
                new Claim(ClaimTypes.Role, roles.FirstOrDefault())
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
        {
            Token = tokenHandler.WriteToken(token),
            User = _mapper.Map<UserDTO>(user)
        };
        return loginResponseDTO;

    }

    public async Task<UserDTO> Register(RegisterationRequestDTO registerationRequestDTO)
    {
        ApplicationUser user = new()
        {
            UserName = registerationRequestDTO.UserName,
            Email = registerationRequestDTO.UserName,
            NormalizedEmail = registerationRequestDTO.UserName.ToUpper(),
            Name = registerationRequestDTO.Name
        };

        try
        {
            IdentityResult result = await _userManager.CreateAsync(user, registerationRequestDTO.Password);
            if (result.Succeeded)
            {
                if (!(await _roleManager.RoleExistsAsync("admin")))
                {
                    await _roleManager.CreateAsync(new IdentityRole("admin"));
                    await _roleManager.CreateAsync(new IdentityRole("admin"));
                }

                await _userManager.AddToRoleAsync(user, "admin");
                ApplicationUser userToReturn = _db.ApplicationUsers
                    .FirstOrDefault(u => u.UserName == registerationRequestDTO.UserName);
                return _mapper.Map<UserDTO>(userToReturn);
            }
        }
        catch (Exception e)
        {

        }

        return new UserDTO();
    }
}

