using System.Net;
using System.Text.Json;
using Repom.Domain.Exceptions;

namespace Repom.API.Middlewares;

public class ExceptionHandlingMiddleware {
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger) {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context) {
        try {
            await _next(context);
        }
        catch (Exception ex) {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception) {
        context.Response.ContentType = "application/json";

        var (statusCode, title) = exception switch {
            DomainException => (HttpStatusCode.BadRequest, "Regra de negócio violada"),
            KeyNotFoundException => (HttpStatusCode.NotFound, "Recurso não encontrado"),
            _ => (HttpStatusCode.InternalServerError, "Erro interno no servidor")
        };

        if (statusCode == HttpStatusCode.InternalServerError)
            _logger.LogError(exception, "Erro não tratado capturado pelo middleware.");

        context.Response.StatusCode = (int)statusCode;

        var response = new {
            status = (int)statusCode,
            title,
            detail = exception.Message
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}