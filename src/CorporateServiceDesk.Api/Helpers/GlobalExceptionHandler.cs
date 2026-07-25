using CorporateServiceDesk.Application.Common.Exceptions;
using CorporateServiceDesk.Domain.Common.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CorporateServiceDesk.Api.Helpers
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IProblemDetailsService _problemDetailsService;
        private readonly IHostEnvironment _environment;

        public GlobalExceptionHandler(
            ILogger<GlobalExceptionHandler> logger,
            IProblemDetailsService problemDetailsService,
            IHostEnvironment environment)
        {
            _logger = logger;
            _problemDetailsService = problemDetailsService;
            _environment = environment;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            (int statusCode, string title) = GetExceptionDetails(exception);

            _logger.LogError(
                exception,
                "Erro durante o processamento de {Method} {Path}. TraceId: {TraceId}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                httpContext.TraceIdentifier);

            httpContext.Response.StatusCode = statusCode;

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = GetDetail(exception, statusCode),
                Instance = httpContext.Request.Path
            };

            problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

            if (_environment.IsDevelopment())
            {
                problemDetails.Extensions["exceptionType"] = exception.GetType().Name;
                problemDetails.Extensions["stackTrace"] = exception.StackTrace;
            }

            return await _problemDetailsService.TryWriteAsync(
                new ProblemDetailsContext
                {
                    HttpContext = httpContext,
                    ProblemDetails = problemDetails,
                    Exception = exception
                });
        }

        private string GetDetail(Exception exception, int statusCode)
        {
            if (_environment.IsDevelopment() ||
                statusCode != StatusCodes.Status500InternalServerError)
            {
                return exception.Message;
            }

            return "Ocorreu um erro interno ao processar a solicitação.";
        }

        private static (int StatusCode, string Title) GetExceptionDetails(
            Exception exception)
        {
            return exception switch
            {
                ArgumentNullException =>
                    (StatusCodes.Status400BadRequest, "Requisição inválida"),

                ArgumentException =>
                    (StatusCodes.Status400BadRequest, "Requisição inválida"),

                UnauthorizedAccessException =>
                    (StatusCodes.Status403Forbidden, "Acesso não autorizado"),

                ConflictException =>
                    (StatusCodes.Status409Conflict, "Conflito na operação"),

                NotFoundException =>
                    (StatusCodes.Status404NotFound, "Recurso não encontrado"),

                DomainException =>
                    (StatusCodes.Status400BadRequest, "Regra de negócio inválida"),

                _ =>
                    (StatusCodes.Status500InternalServerError, "Erro interno do servidor")
            };
        }
    }
}

