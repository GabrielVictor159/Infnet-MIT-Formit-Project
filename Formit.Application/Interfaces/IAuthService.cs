using Formit.Shared.DTOs;
using Formit.Shared.DTOs.Requests;

namespace Formit.Application.Interfaces;
public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterUserDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<PagedResultDto<UserResponseDto>> GetAllUsersPagedAsync(int page, int pageSize, string? searchTerm, string currentUserId);
    Task<UserResponseDto> GetCurrentUserAsync();
    Task<IEnumerable<string>> GetAllRolesAsync();
    Task<UserResponseDto> GetUserByIdAsync(string id);
    Task<AuthResponseDto> UpdateUserAsync(string id, AdminUpdateUserDto dto);
    Task<AuthResponseDto> UpdateCurrentUserAsync(UpdateUserDto dto);
    Task<AuthResponseDto> ChangePasswordAsync(ChangePasswordDto dto);
    Task<AuthResponseDto> DeleteUserAsync(string id);
}
