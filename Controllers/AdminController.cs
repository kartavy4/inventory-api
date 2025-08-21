using InventoryApi.Dtos;
using InventoryApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;

namespace InventoryApi.Controllers;

[ApiController]
[Route("api/[controller]")] 
[Authorize(Roles = "Admin")]

public class AdminController : ControllerBase
{
    private readonly UserManager<AppUser>  _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = _userManager.Users.ToList();
        var userDtos = new List<UserDTO>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userDtos.Add(new UserDTO
            {
                Id = user.Id,
                Email = user.Email!,
                Username = user.UserName!,
                Roles = roles
            });
        }
        return Ok(userDtos);
    }

    [HttpGet("users/{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();
        var roles = await _userManager.GetRolesAsync(user);
        var dto = new UserDTO
        {
            Id = user.Id,
            Email = user.Email!,
            Username = user.UserName!,
            Roles = roles
        };
        return Ok(dto);
    }

    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();
        
        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded) return BadRequest(result.Errors);
        // _userManager.UpdateAsync(user);
        return NoContent();
    }

    [HttpPost("users/{id}/add-role")]
    public async Task<IActionResult> AddRole(string id, [FromQuery] string role)
    {
        var user = await _userManager.FindByIdAsync(id);
        if(user == null) return NotFound();

        if (!await _roleManager.RoleExistsAsync(role))
        {
            var roleResult = await _roleManager.CreateAsync(new IdentityRole(role));
            if(!roleResult.Succeeded) return BadRequest(roleResult.Errors);
        }
        
        if (!await _userManager.IsInRoleAsync(user, role))
        {
            var result = await _userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded) return BadRequest(result.Errors);
        }

        return Ok();
    }

    [HttpPost("users/{id}/remove-role")]
    public async Task<IActionResult> RemoveRole(string id, string role)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();
        var result = await _userManager.RemoveFromRoleAsync(user, role);
        if(!result.Succeeded) return BadRequest(result.Errors);
        return Ok();
    }
    
    [HttpPost("assight-role")]
    public async Task<IActionResult> AssightRole(string userEmail, [FromQuery] string role)
    {
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null) return NotFound("User not found");

        var roleExist = await _roleManager.RoleExistsAsync(role);
        if (!roleExist) return NotFound("Role not found");

        var result = await _userManager.AddToRoleAsync(user, role);
        if (!result.Succeeded) return BadRequest(result.Errors);
        return Ok($"Роль {role} присвоена {userEmail}");
    }

    [HttpGet("roles")]
    public IActionResult GetAllRoles()
    {
        var roles = _roleManager.Roles.Select(r=>new {r.Id, r.Name} ).ToList();
        return Ok(roles);
    }
}