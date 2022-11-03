using Boilerplate.Authentication.Models.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Boilerplate.Authentication.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        private const string CONTENT_TYPE = "application/json";

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;

                response.ContentType = CONTENT_TYPE;

                switch (error)
                {
                    case AppException e:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case KeyNotFoundException e:
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    default:
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                context.Request.Body.Position = 0;

                var teste = await new StreamReader(context.Request.Body).ReadToEndAsync();

                var result = JsonSerializer.Serialize(new { message = error?.Message, TraceId = Guid.NewGuid(), teste = teste });

                await response.WriteAsync(result);
            }
        }
    }
}
