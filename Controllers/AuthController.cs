using System.Security.Claims;
using InventoryApi.Dtos;
using InventoryApi.Models;
using InventoryApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly TokenService _tokenService;

    public AuthController(UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        TokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var user = new AppUser { UserName = dto.Username, Email = dto.Email };
        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        await _userManager.AddToRoleAsync(user, "User");
        return Ok("User created");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _userManager.FindByNameAsync(dto.Username);
        if (user == null) return Unauthorized("Invalid username or password");
        
        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password,false);
        if(!result.Succeeded) return Unauthorized("Invalid username or password");
        
        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.CreateToken(user, roles);
        return Ok(token);
    }

    // private string GenerateJwtToken(AppUser user, IList<string> roles)
    // {
    //     var claims = new List<Claim>
    //     {
    //         new(ClaimTypes.NameIdentifier, user.Id),
    //         new(ClaimTypes.Name, user.UserName!),
    //         new(ClaimTypes.Email, user.Email!),
    //     };
    //     foreach (var role in roles){}
    // }
}