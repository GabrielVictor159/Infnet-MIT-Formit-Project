using Formit.Application.Interfaces;
using Formit.Shared.DTOs;
using Formit.Shared.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Formit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet("validate-session")]
    [Authorize]
    public IActionResult ValidateSession()
    {
        return Ok();
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        if (!result.Success)
            return Unauthorized(result);

        return Ok(result);
    }

    [HttpPut("me")]
    [Authorize(Policy = "UserPolicy")] 
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserDto dto)
    {
        var result = await _authService.UpdateCurrentUserAsync(dto);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("change-password")]
    [Authorize(Policy = "UserPolicy")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var result = await _authService.ChangePasswordAsync(dto);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("me")]
    [Authorize(Policy = "UserPolicy")]
    public async Task<IActionResult> GetMyProfile()
    {
        var result = await _authService.GetCurrentUserAsync();

        return Ok(result);
    }

    [HttpGet("users/{id}")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> GetUserById([FromRoute] string id)
    {
        var result = await _authService.GetUserByIdAsync(id);

        return Ok(result);
    }

    [HttpGet("users")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> GetAllUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null)
    {
        if (page < 1)
            page = 1;
        if (pageSize < 1 || pageSize > 50)
            pageSize = 10;

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(currentUserId))
        {
            return Unauthorized(new { message = "User ID not found in token." });
        }

        var result = await _authService.GetAllUsersPagedAsync(page, pageSize, searchTerm, currentUserId);

        return Ok(result);
    }

    [HttpGet("roles")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await _authService.GetAllRolesAsync();
        return Ok(roles);
    }

    [HttpPut("users/{id}")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] AdminUpdateUserDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.UpdateUserAsync(id, dto);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpDelete("users/{id}")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var result = await _authService.DeleteUserAsync(id);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}