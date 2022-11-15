namespace LoginWebApi.Middleware;



public static class LoguearRespuestaHttpMiddlewareExtension
{
    public static IApplicationBuilder UseLoguearRespuesta(this IApplicationBuilder app)
    {
        return app.UseMiddleware<LoguearRespuestaHttpMiddleware>();
    }
}
public class LoguearRespuestaHttpMiddleware
{
    private readonly RequestDelegate siguiente;
    public LoguearRespuestaHttpMiddleware( RequestDelegate siguiente)
    {
        this.siguiente = siguiente;
    }
    
    
    //Invoke o InvokeAsync
    public async Task InvokeAsync(HttpContext contexto, ILogger<LoguearRespuestaHttpMiddleware> logger)
    {
        
    }
    
}