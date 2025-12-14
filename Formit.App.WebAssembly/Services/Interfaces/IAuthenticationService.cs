using Formit.Shared.DTOs;
using Formit.Shared.DTOs.Requests;
using System.Threading.Tasks;

namespace Formit.App.Services.Interfaces;

public interface IAuthenticationService
{
    // --- Fluxo Público ---
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<AuthResponseDto> RegisterAsync(RegisterUserDto registerDto);
    Task Logout();
    Task<bool> IsUserAdminAsync();

    // --- Fluxo do Usuário Logado (Perfil) ---
    Task ValidateSessionAsync();
    Task<UserResponseDto?> GetCurrentUserAsync();
    Task<AuthResponseDto> UpdateProfileAsync(UpdateUserDto updateDto);
    Task<AuthResponseDto> ChangePasswordAsync(ChangePasswordDto passwordDto);

    // --- Fluxo Administrativo ---
    Task<IEnumerable<string>> GetAllRoles();
    Task<UserResponseDto?> GetUserByIdAsync(string id);
    Task<PagedResultDto<UserResponseDto>> GetAllUsersPagedAsync(int page, int pageSize, string? searchItem = null);
    Task<AuthResponseDto> UpdateUserAsync(string id, AdminUpdateUserDto updateDto);
    Task<AuthResponseDto> DeleteUserAsync(string id);
}