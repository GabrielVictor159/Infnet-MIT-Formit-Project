using Formit.Application.Interfaces;
using Formit.Domain.Entities;
using Formit.Shared.DTOs;
using Formit.Shared.DTOs.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Formit.Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AuthService(UserManager<ApplicationUser> userManager,
                       SignInManager<ApplicationUser> signInManager,
                       ICurrentUserService currentUserService,
                       RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _currentUserService = currentUserService;
        _roleManager = roleManager;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterUserDto dto)
    {
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            var errorList = result.Errors.Select(e => e.Description).ToList();

            return new AuthResponseDto(
                Success: false,
                Message: "Error registering user.",
                Token: null,
                UserName: null,
                Errors: errorList
            );
        }

        string roleToAssign = "User";

        if (!string.IsNullOrEmpty(dto.Role))
        {
            bool isCurrentUserAdmin = _currentUserService.Roles.Contains("Admin");

            if (isCurrentUserAdmin)
            {
                if (dto.Role == "Admin" || dto.Role == "User")
                {
                    roleToAssign = dto.Role;
                }
            }
        }

        await _userManager.AddToRoleAsync(user, roleToAssign);

        return new AuthResponseDto(true, "User registered successfully!", null, null);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var result = await _signInManager.PasswordSignInAsync(dto.Email, dto.Password, false, false);

        if (!result.Succeeded)
        {
            return new AuthResponseDto(false, "Invalid email or password.", null, null);
        }

        var user = await _userManager.FindByEmailAsync(dto.Email);

        var roles = await _userManager.GetRolesAsync(user!);

        var token = await GenerateJwtToken(user!);

        return new AuthResponseDto(
            Success: true,
            Message: "Login successful.",
            Token: token,
            UserName: user!.FullName,
            Roles: roles,
            Errors: null
        );
    }

    public async Task<PagedResultDto<UserResponseDto>> GetAllUsersPagedAsync(int page, int pageSize, string? searchTerm, string currentUserId)
    {
        var query = _userManager.Users.AsNoTracking();

        if (!string.IsNullOrEmpty(currentUserId))
        {
            query = query.Where(u => u.Id != currentUserId);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(u => u.FullName.Contains(searchTerm) || u.Email.Contains(searchTerm));
        }

        var totalCount = await query.CountAsync();

        var pagedUsers = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var userDtos = new List<UserResponseDto>();

        foreach (var user in pagedUsers)
        {
            var roles = await _userManager.GetRolesAsync(user);

            userDtos.Add(new UserResponseDto(

                Id: user.Id,
                FullName: user.FullName,
                Email: user.Email!,
                UserName: user.UserName!,
                Roles: roles
            ));
        }

        return new PagedResultDto<UserResponseDto>
        {
            Items = userDtos,
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = pageSize
        };
    }

    public async Task<UserResponseDto> GetCurrentUserAsync()
    {
        var userId = _currentUserService.Id;

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User not identified.");

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            throw new KeyNotFoundException("User not found in the database.");

        var roles = await _userManager.GetRolesAsync(user);

        return new UserResponseDto(
            user.Id,
            user.FullName,
            user.Email!,
            user.UserName!,
            roles
        );
    }

    public async Task<AuthResponseDto> UpdateCurrentUserAsync(UpdateUserDto dto)
    {
        var userId = _currentUserService.Id;
        if (string.IsNullOrEmpty(userId))
            return new AuthResponseDto(false, "User not identified.", null, null);

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return new AuthResponseDto(false, "User not found.", null, null);

        user.FullName = dto.FullName;
        user.Email = dto.Email;
        user.UserName = dto.Email;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return new AuthResponseDto(false, "Error updating profile.", null, null, errors);
        }

        var newToken = await GenerateJwtToken(user);

        return new AuthResponseDto(true, "Profile updated successfully!", newToken, user.FullName);
    }

    public async Task<AuthResponseDto> ChangePasswordAsync(ChangePasswordDto dto)
    {
        var userId = _currentUserService.Id;
        if (string.IsNullOrEmpty(userId))
            return new AuthResponseDto(false, "User not identified.", null, null);

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return new AuthResponseDto(false, "User not found.", null, null);

        var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return new AuthResponseDto(false, "Error changing password.", null, null, errors);
        }

        return new AuthResponseDto(true, "Password changed successfully!", null, null);
    }

    public async Task<IEnumerable<string>> GetAllRolesAsync()
    {
        return await _roleManager.Roles
            .Select(r => r.Name!)
            .ToListAsync();
    }
    public async Task<AuthResponseDto> UpdateUserAsync(string id, AdminUpdateUserDto dto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return new AuthResponseDto(false, "User not found.", null, null);

        foreach (var role in dto.Roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                return new AuthResponseDto(false, $"Role '{role}' does not exist.", null, null);
            }
        }

        user.FullName = dto.FullName;
        user.Email = dto.Email;
        user.UserName = dto.Email; 

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            var errors = updateResult.Errors.Select(e => e.Description).ToList();
            return new AuthResponseDto(false, "Error updating user information.", null, null, errors);
        }

        var currentRoles = await _userManager.GetRolesAsync(user);

        var rolesToAdd = dto.Roles.Except(currentRoles).ToList();

        var rolesToRemove = currentRoles.Except(dto.Roles).ToList();

        if (rolesToAdd.Any())
        {
            var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
            if (!addResult.Succeeded)
                return new AuthResponseDto(false, "User updated, but failed to add roles.", null, null);
        }

        if (rolesToRemove.Any())
        {
            var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            if (!removeResult.Succeeded)
                return new AuthResponseDto(false, "User updated, but failed to remove old roles.", null, null);
        }

        return new AuthResponseDto(true, "User and roles updated successfully!", null, user.FullName);
    }

    public async Task<AuthResponseDto> DeleteUserAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return new AuthResponseDto(false, "User not found.", null, null);

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return new AuthResponseDto(false, "Error deleting user.", null, null, errors);
        }

        return new AuthResponseDto(true, "User deleted successfully.", null, null);
    }

    private async Task<string> GenerateJwtToken(ApplicationUser user)
    {
        var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET_KEY")!);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim("FullName", user.FullName)
        };

        var roles = await _userManager.GetRolesAsync(user);

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(int.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRATION_HOURS")!)),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
            Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public async Task<UserResponseDto> GetUserByIdAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
            throw new UnauthorizedAccessException("User not identified.");

        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            throw new KeyNotFoundException("User not found in the database.");

        var roles = await _userManager.GetRolesAsync(user);

        return new UserResponseDto(
            user.Id,
            user.FullName,
            user.Email!,
            user.UserName!,
            roles
        );
    }
}