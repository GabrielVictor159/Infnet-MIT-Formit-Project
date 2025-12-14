using Blazored.LocalStorage;
using Formit.App.Services.Interfaces;
using Formit.Shared.DTOs;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Net.Http.Json;
using System.Text.Json;

namespace Formit.App.Services;

public class OptionService : BaseService, IOptionService
{
    public OptionService(HttpClient httpClient, NavigationManager navigationManager, ILocalStorageService localStorage, ISnackbar snackbar, JsonSerializerOptions options)
        : base(httpClient, navigationManager, localStorage, snackbar, options)
    {
    }

    public async Task<OptionResponseDto> CreateAsync(int questionId, CreateOptionDto dto)
    {
        var result = await ExecuteSafeAsync(async () =>
        {
            await PrepareRequestAsync();
            var response = await _httpClient.PostAsJsonAsync($"api/Option/question/{questionId}", dto);

            await HandleResponseAsync(response);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<OptionResponseDto>(_options);
            }
            return null;
        });

        return result ?? throw new HttpRequestException("Error handled globally");
    }

    public async Task<OptionResponseDto> UpdateAsync(int id, UpdateOptionDto dto)
    {
        var result = await ExecuteSafeAsync(async () =>
        {
            await PrepareRequestAsync();
            var response = await _httpClient.PutAsJsonAsync($"api/Option/{id}", dto);

            await HandleResponseAsync(response);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<OptionResponseDto>(_options);
            }
            return null;
        });

        return result ?? throw new HttpRequestException("Error handled globally");
    }

    public async Task DeleteAsync(int id)
    {
        await ExecuteSafeAsync(async () =>
        {
            await PrepareRequestAsync();
            var response = await _httpClient.DeleteAsync($"api/Option/{id}");
            await HandleResponseAsync(response);
        });
    }
}