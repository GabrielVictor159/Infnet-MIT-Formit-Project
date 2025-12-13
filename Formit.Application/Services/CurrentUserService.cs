using Formit.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Formit.Application.Services;
public class CurrentUser : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public string Id => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

    public string UserName => User?.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;

    public string Email => User?.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
    public string FullName => User?.FindFirst("FullName")?.Value ?? string.Empty;

    public IEnumerable<string> Roles => User?.FindAll(ClaimTypes.Role).Select(c => c.Value) ?? new List<string>();

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public string GetClaim(string claimType) => User?.FindFirst(claimType)?.Value ?? string.Empty;
}
