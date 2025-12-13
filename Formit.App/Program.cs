using Blazored.LocalStorage;
using Formit.App.Components;
using Formit.App.Services;
using Formit.App.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using MudBlazor.Services;
using System.Text.Json;
using Microsoft.Extensions.Http.Resilience;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddHubOptions(options =>
    {
        options.MaximumReceiveMessageSize = 10 * 1024 * 1024;

        options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
        options.HandshakeTimeout = TimeSpan.FromSeconds(30);
    })
    .AddCircuitOptions(options => options.DetailedErrors = true);

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = true;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 5000;
});


builder.Services.AddBlazoredLocalStorage();
builder.Services.AddSingleton(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "Cookies";
})
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/login";
        options.ExpireTimeSpan = TimeSpan.FromDays(1);
    });

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

var apiUrlString = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:8081/";
var apiBaseUrl = new Uri(apiUrlString);

Action<HttpClient> configureClient = client =>
{
    client.BaseAddress = apiBaseUrl;
};

Func<HttpMessageHandler> configureHandler = () =>
{
    return new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
    };
};

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

builder.Services.AddScoped<CustomAuthenticationStateProvider>(provider =>
    (CustomAuthenticationStateProvider)provider.GetRequiredService<AuthenticationStateProvider>());

builder.Services.AddHttpClient<IAuthenticationService, AuthenticationService>(configureClient)
    .ConfigurePrimaryHttpMessageHandler(configureHandler)
    .AddStandardResilienceHandler();

builder.Services.AddHttpClient<IQuizService, QuizService>(configureClient)
    .ConfigurePrimaryHttpMessageHandler(configureHandler)
    .AddStandardResilienceHandler();

builder.Services.AddHttpClient<IQuestionService, QuestionService>(configureClient)
    .ConfigurePrimaryHttpMessageHandler(configureHandler)
    .AddStandardResilienceHandler();

builder.Services.AddHttpClient<IOptionService, OptionService>(configureClient)
    .ConfigurePrimaryHttpMessageHandler(configureHandler)
    .AddStandardResilienceHandler();

builder.Services.AddHttpClient<ISubmissionService, SubmissionService>(configureClient)
    .ConfigurePrimaryHttpMessageHandler(configureHandler)
    .AddStandardResilienceHandler();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
