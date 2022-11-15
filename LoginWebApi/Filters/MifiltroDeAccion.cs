using Microsoft.AspNetCore.Mvc.Filters;

namespace LoginWebApi.Filters;

public class MifiltroDeAccion : IActionFilter
{
    public readonly ILogger<MifiltroDeAccion> Logger;

    public MifiltroDeAccion(ILogger<MifiltroDeAccion> Logger)
    {
        this.Logger = Logger;
    }
    
    public void OnActionExecuting(ActionExecutingContext context)
    {
        Logger.LogInformation("Antes de ejecutar la accion :-)");
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
       Logger.LogInformation("Despues de ejecutar la acción :-(");
    }
}