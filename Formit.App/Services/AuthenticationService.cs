using Blazored.LocalStorage;
using Formit.App.Services.Interfaces;
using Formit.Shared.DTOs;
using Formit.Shared.DTOs.Requests;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Net.Http.Json;
using System.Text.Json;

namespace Formit.App.Services;

public class AuthenticationService : BaseService, IAuthenticationService
{
    public AuthenticationService(HttpClient httpClient, NavigationManager navigationManager, ILocalStorageService localStorage, ISnackbar snackbar, JsonSerializerOptions options)
        : base(httpClient, navigationManager, localStorage, snackbar, options)
    {
    }

    public async Task ValidateSessionAsync()
    {
        await ExecuteSafeAsync(async () =>
        {
            await PrepareRequestAsync();
            var response = await _httpClient.GetAsync("api/Auth/validate-session");

            await HandleResponseAsync(response);
        });
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        return await ExecuteSafeAsync(async () =>
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/login", loginDto);

            await HandleResponseAsync(response);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>(_options);

                if (result != null && result.Success && !string.IsNullOrEmpty(result.Token))
                {
                    await _localStorage.SetItemAsync("authToken", result.Token);
                    await _localStorage.SetItemAsync("userRoles", result.Roles);
                    _navigationManager.NavigateTo("/");
                    return result;
                }
            }

            _snackbar.Add("Incorrect email or password.", Severity.Error);

            return new AuthResponseDto(false, "Login failed", null, null);

        }) ?? new AuthResponseDto(false, "Login failed", null, null);
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterUserDto registerDto)
    {
        return await ExecuteSafeAsync(async () =>
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/register", registerDto);

            await HandleResponseAsync(response);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AuthResponseDto>(_options)
                       ?? new AuthResponseDto(true, "Registration successful", null, null);
            }

            return new AuthResponseDto(false, "Registration failed", null, null);

        }) ?? new AuthResponseDto(false, "Registration failed", null, null);
    }

    public async Task<AuthResponseDto> UpdateProfileAsync(UpdateUserDto updateDto)
    {
        return await ExecuteSafeAsync(async () =>
        {
            await PrepareRequestAsync();
            var response = await _httpClient.PutAsJsonAsync("api/Auth/me", updateDto);

            await HandleResponseAsync(response);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AuthResponseDto>(_options)
                       ?? new AuthResponseDto(true, "Updated", null, null);
            }

            return new AuthResponseDto(false, "Update failed", null, null);

        }) ?? new AuthResponseDto(false, "Update failed", null, null);
    }

    public async Task<AuthResponseDto> ChangePasswordAsync(ChangePasswordDto passwordDto)
    {
        return await ExecuteSafeAsync(async () =>
        {
            await PrepareRequestAsync();
            var response = await _httpClient.PostAsJsonAsync("api/Auth/change-password", passwordDto);

            await HandleResponseAsync(response);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AuthResponseDto>(_options)
                       ?? new AuthResponseDto(true, "Changed", null, null);
            }

            return new AuthResponseDto(false, "Change failed", null, null);

        }) ?? new AuthResponseDto(false, "Change failed", null, null);
    }

    public async Task<UserResponseDto?> GetCurrentUserAsync()
    {
        return await ExecuteSafeAsync(async () =>
        {
            await PrepareRequestAsync();
            var response = await _httpClient.GetAsync("api/Auth/me");

            await HandleResponseAsync(response);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserResponseDto>(_options);
            }
            return null;
        });
    }

    public async Task<PagedResultDto<UserResponseDto>> GetAllUsersPagedAsync(int page, int pageSize, string? searchItem = null)
    {
        return await ExecuteSafeAsync(async () =>
        {
            await PrepareRequestAsync();

            var url = $"api/Auth/users?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(searchItem))
            {
                url += $"&searchTerm={Uri.EscapeDataString(searchItem)}";
            }

            var response = await _httpClient.GetAsync(url);

            await HandleResponseAsync(response);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<PagedResultDto<UserResponseDto>>(_options);
            }

            return new PagedResultDto<UserResponseDto>();

        }) ?? new PagedResultDto<UserResponseDto>();
    }

    public async Task<AuthResponseDto> UpdateUserAsync(string id, AdminUpdateUserDto updateDto)
    {
        return await ExecuteSafeAsync(async () =>
        {
            await PrepareRequestAsync();
            var response = await _httpClient.PutAsJsonAsync($"api/Auth/users/{id}", updateDto);

            await HandleResponseAsync(response);

            if (response.IsSuccessStatusCode)
            {
                return new AuthResponseDto(true, "Success", null, null);
            }
            return new AuthResponseDto(false, "Failed", null, null);

        }) ?? new AuthResponseDto(false, "Failed", null, null);
    }

    public async Task<AuthResponseDto> DeleteUserAsync(string id)
    {
        return await ExecuteSafeAsync(async () =>
        {
            await PrepareRequestAsync();
            var response = await _httpClient.DeleteAsync($"api/Auth/users/{id}");

            await HandleResponseAsync(response);

            if (response.IsSuccessStatusCode)
            {
                return new AuthResponseDto(true, "Success", null, null);
            }
            return new AuthResponseDto(false, "Failed", null, null);

        }) ?? new AuthResponseDto(false, "Failed", null, null);
    }

    public async Task<IEnumerable<string>> GetAllRoles()
    {
        return await ExecuteSafeAsync(async () =>
        {
            await PrepareRequestAsync();
            var response = await _httpClient.GetAsync("api/Auth/roles");

            await HandleResponseAsync(response);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<string>>(_options);
            }

            return new List<string>();
        }) ?? new List<string>();
    }

    public async Task<UserResponseDto?> GetUserByIdAsync(string id)
    {
        return await ExecuteSafeAsync(async () =>
        {
            await PrepareRequestAsync();
            var response = await _httpClient.GetAsync($"api/Auth/users/{id}");

            await HandleResponseAsync(response);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserResponseDto>(_options);
            }
            return null;
        });
    }

    public async Task Logout()
    { 
        await _localStorage.RemoveItemAsync("authToken");
        await _localStorage.RemoveItemAsync("userRoles");

        _navigationManager.NavigateTo("/login");
    }

}