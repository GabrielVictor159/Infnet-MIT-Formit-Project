using Blazored.LocalStorage;
using Formit.App.Services.Interfaces;
using Formit.Shared.DTOs;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Net.Http.Json;
using System.Text.Json;

namespace Formit.App.Services;

public class SubmissionService : BaseService, ISubmissionService
{
    public SubmissionService(HttpClient httpClient, NavigationManager navigationManager, ILocalStorageService localStorage, ISnackbar snackbar, JsonSerializerOptions options)
        : base(httpClient, navigationManager, localStorage, snackbar, options)
    {
    }
    private record SubmitResponseWrapper(int SubmissionId, string Message);

    public async Task<int> SubmitQuizAsync(SubmitFormDto dto)
    {
        var result = await ExecuteSafeAsync(async () =>
        {
            await PrepareRequestAsync();
            var response = await _httpClient.PostAsJsonAsync("api/Submission", dto);

            await HandleResponseAsync(response);

            if (response.IsSuccessStatusCode)
            {
                var wrapper = await response.Content.ReadFromJsonAsync<SubmitResponseWrapper>(_options);
                return wrapper?.SubmissionId ?? 0;
            }
            return 0;
        });

        if (result == 0)
            throw new HttpRequestException("Error handled globally");
        return result;
    }

    public async Task<IEnumerable<SubmissionSummaryDto>> GetSubmissionsByQuizIdAsync(int quizId)
    {
        return await ExecuteSafeAsync(async () =>
        {
            await PrepareRequestAsync();
            var response = await _httpClient.GetAsync($"api/Submission/quiz/{quizId}");

            await HandleResponseAsync(response);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<SubmissionSummaryDto>>(_options)
                       ?? Enumerable.Empty<SubmissionSummaryDto>();
            }
            return Enumerable.Empty<SubmissionSummaryDto>();

        }) ?? Enumerable.Empty<SubmissionSummaryDto>();
    }

    public async Task<SubmissionDetailsDto?> GetSubmissionDetailsAsync(int submissionId)
    {
        return await ExecuteSafeAsync(async () =>
        {
            await PrepareRequestAsync();
            var response = await _httpClient.GetAsync($"api/Submission/{submissionId}");

            await HandleResponseAsync(response);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<SubmissionDetailsDto>(_options);
            }
            return null;
        });
    }
}