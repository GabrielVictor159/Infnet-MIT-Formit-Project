using Blazored.LocalStorage;
using Formit.App.Services.Interfaces;
using Formit.Shared.DTOs;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Net.Http.Json;
using System.Text.Json;

namespace Formit.App.Services;

public class QuestionService : BaseService, IQuestionService
{
    public QuestionService(HttpClient httpClient, NavigationManager navigationManager, ILocalStorageService localStorage, ISnackbar snackbar, JsonSerializerOptions options)
        : base(httpClient, navigationManager, localStorage, snackbar, options)
    {
    }

    public async Task<QuestionResponseDto?> GetByIdAsync(int id)
    {
        return await ExecuteSafeAsync(async () =>
        {
            await PrepareRequestAsync();
            var response = await _httpClient.GetAsync($"api/Question/{id}");

            await HandleResponseAsync(response);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<QuestionResponseDto>(_options);
            }
            return null;
        });
    }

    public async Task<QuestionResponseDto> CreateAsync(int quizId, CreateQuestionDto dto)
    {
        var result = await ExecuteSafeAsync(async () =>
        {
            await PrepareRequestAsync();
            var response = await _httpClient.PostAsJsonAsync($"api/Question/quiz/{quizId}", dto);

            await HandleResponseAsync(response);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<QuestionResponseDto>(_options);
            }
            return null;
        });

        return result ?? throw new HttpRequestException("Error handled globally");
    }

    public async Task<QuestionResponseDto> UpdateAsync(int id, UpdateQuestionDto dto)
    {
        var result = await ExecuteSafeAsync(async () =>
        {
            await PrepareRequestAsync();
            var response = await _httpClient.PutAsJsonAsync($"api/Question/{id}", dto);

            await HandleResponseAsync(response);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<QuestionResponseDto>(_options);
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
            var response = await _httpClient.DeleteAsync($"api/Question/{id}");
            await HandleResponseAsync(response);
        });
    }
}