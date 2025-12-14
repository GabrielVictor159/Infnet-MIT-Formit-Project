using Blazored.LocalStorage;
using Formit.Api.Middlewares;
using Formit.Shared.DTOs;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Net.Http.Headers;
using System.Net;
using System.Text.Json;

namespace Formit.App.Services;

public abstract class BaseService
{
    protected readonly HttpClient _httpClient;
    protected readonly NavigationManager _navigationManager;
    protected readonly ILocalStorageService _localStorage;
    protected readonly ISnackbar _snackbar;

    protected readonly JsonSerializerOptions _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

    public BaseService(HttpClient httpClient, NavigationManager navigationManager, ILocalStorageService localStorage, ISnackbar snackbar, JsonSerializerOptions options)
    {
        _httpClient = httpClient;
        _navigationManager = navigationManager;
        _localStorage = localStorage;
        _snackbar = snackbar;
        _options = options;
    }

    protected async Task<T?> ExecuteSafeAsync<T>(Func<Task<T?>> action)
    {
        try
        {
            return await action();
        }
        catch (Exception ex)
        {
            HandleException(ex);
            return default;
        }
    }
    protected async Task<bool> ExecuteSafeAsync(Func<Task> action)
    {
        try
        {
            await PrepareRequestAsync();
            await action();
            return true;
        }
        catch (Exception ex)
        {
            HandleException(ex);
            return false;
        }
    }

    protected async Task<HttpClient> PrepareRequestAsync()
    {
        try
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            return _httpClient;
        }
        catch
        {
            return _httpClient;
        }
    }

    protected async Task HandleResponseAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
        {
            var uri = _navigationManager.Uri.ToLower();
            if (!uri.Contains("/login") && !uri.Contains("/register"))
            {
                await _localStorage.RemoveItemAsync("authToken");
                await _localStorage.RemoveItemAsync("userRoles");
                _navigationManager.NavigateTo("/login");
                _snackbar.Add("Session expired.", Severity.Warning);
            }
            return;
        }

        var content = await response.Content.ReadAsStringAsync();
        try
        {
            var authError = JsonSerializer.Deserialize<AuthResponseDto>(content, _options);

            if (authError != null && !authError.Success)
            {
                bool errorFound = false;

                if (!string.IsNullOrEmpty(authError.Message))
                {
                    _snackbar.Add(authError.Message, Severity.Error);
                    errorFound = true;
                }

                if (authError.Errors != null)
                {
                    foreach (var err in authError.Errors)
                    {
                        _snackbar.Add(err, Severity.Error);
                    }
                    errorFound = true;
                }

                if (authError.Roles != null)
                {
                    foreach (var roleError in authError.Roles)
                    {
                        _snackbar.Add(roleError, Severity.Error);
                    }
                    errorFound = true;
                }

                if (errorFound)
                    return;
            }

            var apiError = JsonSerializer.Deserialize<ErrorDetails>(content, _options);
            if (apiError != null && !string.IsNullOrEmpty(apiError.Message))
            {
                _snackbar.Add(apiError.Message, Severity.Error);
                return;
            }
        }
        catch { }

        _snackbar.Add($"Request failed: {response.ReasonPhrase}", Severity.Error);
    }

    protected void HandleException(Exception ex)
    {
        Console.WriteLine(ex.Message);
        _snackbar.Add("Connection error. Server is unreachable.", Severity.Error);
    }
}
