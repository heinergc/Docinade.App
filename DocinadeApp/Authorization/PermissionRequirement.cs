using Microsoft.AspNetCore.Authorization;

namespace DocinadeApp.Authorization
{
    /// <summary>
    /// Requisito de autorización para permisos específicos
    /// </summary>
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }

        public PermissionRequirement(string permission)
        {
            Permission = permission ?? throw new ArgumentNullException(nameof(permission));
        }
    }
}