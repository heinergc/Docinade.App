using Microsoft.AspNetCore.Authorization;

namespace RubricasApp.Web.Authorization
{
    /// <summary>
    /// Atributo para requerir un permiso específico
    /// </summary>
    public class RequirePermissionAttribute : AuthorizeAttribute
    {
        public RequirePermissionAttribute(string permission)
        {
            Policy = permission;
        }
    }
}