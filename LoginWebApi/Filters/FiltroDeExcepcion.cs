using Microsoft.AspNetCore.Mvc.Filters;

namespace LoginWebApi.Filters;

public class FiltroDeExcepcion: ExceptionFilterAttribute
{
    private readonly ILogger<FiltroDeExcepcion> Logger;

    public FiltroDeExcepcion(ILogger<FiltroDeExcepcion> logger)
    {
        this.Logger = logger;
    }

    public override void OnException(ExceptionContext context)
    {
        Logger.LogError(context.Exception, context.Exception.Message);
        base.OnException(context);
    }

}