using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Thunders.Api.Filters
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var error = new Error();
            var statusCode = 0;
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message.ToString());

                using (var reader = new System.IO.StreamReader(context.Request.Body))
                {
                    await reader.ReadToEndAsync();
                }

                if (ex is ValidationException validationException)
                {
                    if (validationException.Errors?.Count() > 0)
                    {
                        statusCode = 400;
                        error.message = string.Join(" - ", validationException.Errors.Select(e => e.ErrorMessage));
                    }
                    else
                    {
                        statusCode = 400;
                        error.message = ex.Message;
                    }
                }
                else if (ex is Domain.Base.BasicException basicException)
                {
                    statusCode = basicException.Code;
                    error.message = basicException.Message;
                }
                else
                {
                    statusCode = 500;
                    error.message = $"Ocorreu um erro durante o processo, por gentileza tente novamente mais tarde.{(string.IsNullOrWhiteSpace(ex.Message) ? "" : " (" + ex.Message + ")")}";
                }

                error.status = statusCode;

                var _objserialized = JsonSerializer.Serialize(error);

                context.Response.Clear();
                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(_objserialized);
            }
        }
    }
}
