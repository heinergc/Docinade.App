using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RubricasApp.Web.Services.Identity;
using RubricasApp.Web.Interfaces;

namespace RubricasApp.Web.Filters
{
    /// <summary>
    /// Filtro para validar que el usuario puede acceder a una entidad específica
    /// </summary>
    public class UserOwnershipFilter : IAsyncActionFilter
    {
        private readonly IUserContextService _userContextService;

        public UserOwnershipFilter(IUserContextService userContextService)
        {
            _userContextService = userContextService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Buscar parámetros que sean entidades con control de usuario
            foreach (var parameter in context.ActionArguments)
            {
                if (parameter.Value is IUserOwned entity)
                {
                    if (!await _userContextService.CanAccessEntityAsync(entity))
                    {
                        context.Result = new ForbidResult();
                        return;
                    }
                }
            }

            await next();
        }
    }

    /// <summary>
    /// Atributo para aplicar el filtro de propiedad de usuario
    /// </summary>
    public class RequireUserOwnershipAttribute : TypeFilterAttribute
    {
        public RequireUserOwnershipAttribute() : base(typeof(UserOwnershipFilter))
        {
        }
    }
}
