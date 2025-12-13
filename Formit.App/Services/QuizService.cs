using Blazored.LocalStorage;
using Formit.App.Services.Interfaces;
using Formit.Shared.DTOs;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Net.Http.Json;
using System.Text.Json;

namespace Formit.App.Services;

public class QuizService : BaseService, IQuizService
{
    public QuizService(HttpClient httpClient, NavigationManager navigationManager, ILocalStorageService localStorage, ISnackbar snackbar, JsonSerializerOptions options)
        : base(httpClient, navigationManager, localStorage, snackbar, options)
    {
    }

    public async Task<PagedResultDto<QuizResponseDto>> GetAllPagedAsync(int page, int pageSize, string? title = null)
    {
        return await ExecuteSafeAsync(async () =>
        {
            await PrepareRequestAsync();

            var url = $"api/quiz?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(title))
            {
                url += $"&title={Uri.EscapeDataString(title)}";
            }

            var response = await _httpClient.GetAsync(url);

            await HandleResponseAsync(response);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<PagedResultDto<QuizResponseDto>>(_options);
            }

            return new PagedResultDto<QuizResponseDto>();

        }) ?? new PagedResultDto<QuizResponseDto>();
    }

    public async Task<QuizResponseDto?> GetByIdAsync(int id)
    {
        return await ExecuteSafeAsync(async () =>
        {
            await PrepareRequestAsync();
            var response = await _httpClient.GetAsync($"api/Quiz/{id}");

            await HandleResponseAsync(response);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<QuizResponseDto>(_options);
            }
            return null;
        });
    }

    public async Task<QuizResponseDto> CreateAsync(CreateQuizDto dto)
    {
        var result = await ExecuteSafeAsync(async () =>
        {
            await PrepareRequestAsync();
            var response = await _httpClient.PostAsJsonAsync("api/Quiz", dto);

            await HandleResponseAsync(response);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<QuizResponseDto>(_options);
            }
            return null;
        });

        return result ?? throw new HttpRequestException("Error handled globally");
    }

    public async Task<QuizResponseDto> UpdateAsync(int id, UpdateQuizDto dto)
    {
        var result = await ExecuteSafeAsync(async () =>
        {
            await PrepareRequestAsync();
            var response = await _httpClient.PutAsJsonAsync($"api/Quiz/{id}", dto);

            await HandleResponseAsync(response);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<QuizResponseDto>(_options);
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
            var response = await _httpClient.DeleteAsync($"api/Quiz/{id}");
            await HandleResponseAsync(response);
        });
    }
}