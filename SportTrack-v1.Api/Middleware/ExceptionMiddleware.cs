using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SportTrack_v1.Controladores.Exceptions;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace SportTrack_v1.Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context, SportTrack_v1.Controladores.Audit.IAuditService auditService)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await auditService.RegistrarErrorAsync(ex, context.Request.Path);
                
                context.Response.ContentType = "application/json";
                
                context.Response.StatusCode = ex switch
                {
                    NotFoundException => (int)HttpStatusCode.NotFound,
                    UnauthorizedException => (int)HttpStatusCode.Unauthorized,
                    BadRequestException => (int)HttpStatusCode.BadRequest,
                    _ => (int)HttpStatusCode.InternalServerError
                };

                var response = new
                {
                    statusCode = context.Response.StatusCode,
                    message = ex.Message,
                    innerMessage = _env.IsDevelopment() ? GetFullInnerException(ex) : null,
                    details = _env.IsDevelopment() ? ex.StackTrace?.ToString() : null
                };

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }
        }

        private string? GetFullInnerException(Exception ex)
        {
            if (ex.InnerException == null) return null;
            var inner = ex.InnerException;
            var message = inner.Message;
            while (inner.InnerException != null)
            {
                inner = inner.InnerException;
                message += " --> " + inner.Message;
            }
            return message;
        }
    }
}
