using System.Security.Claims;

namespace RubricasApp.Web.Services.Identity
{
    public interface IUserContextService
    {
        string? GetCurrentUserId();
        ClaimsPrincipal? GetCurrentUser();
        Task<bool> IsInRoleAsync(string role);
        Task<bool> CanAccessEntityAsync<T>(T entity) where T : class;
        Task<bool> CanEditEntityAsync<T>(T entity) where T : class;
        Task<bool> CanDeleteEntityAsync<T>(T entity) where T : class;
    }
}
