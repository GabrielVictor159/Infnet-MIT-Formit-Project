using System.Text.Json;
#pragma warning disable CA1416
using Blazored.LocalStorage;
using Formit.App.Services;
using Formit.App.Services.Interfaces;
using Formit.App.WebAssembly;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

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

builder.Services.AddAuthorizationCore();

var apiUrlString = builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:8084/";
var apiBaseUrl = new Uri(apiUrlString);

Action<HttpClient> configureClient = client =>
{
    client.BaseAddress = apiBaseUrl;
};

builder.Services.AddHttpClient<IAuthenticationService, AuthenticationService>(configureClient)
    .AddStandardResilienceHandler();

builder.Services.AddHttpClient<IQuizService, QuizService>(configureClient)
    .AddStandardResilienceHandler();

builder.Services.AddHttpClient<IQuestionService, QuestionService>(configureClient)
    .AddStandardResilienceHandler();

builder.Services.AddHttpClient<IOptionService, OptionService>(configureClient)
    .AddStandardResilienceHandler();

builder.Services.AddHttpClient<ISubmissionService, SubmissionService>(configureClient)
    .AddStandardResilienceHandler();

await builder.Build().RunAsync();


#pragma warning restore CA1416 