using MSGraphShield.Proxy.Exceptions;
using MSGraphShield.Proxy.Exceptions.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Titanium.Web.Proxy.EventArguments;

namespace MSGraphShield.Proxy.Plumbings.Pipeline
{
    internal static class PipelineExtensions
    {
        /// <summary>
        /// Handles exceptions occurring during the execution of pipeline steps.
        /// </summary>
        public static async Task HandleExceptionAsync(this SessionEventArgs session, Func<Task> action, ILogger logger)
        {
            try
            {
                await action();
            }
            catch (Exception error)
            {
                session.HandleException(error, logger);
            }
        }

        private static void HandleException(this SessionEventArgs session, Exception error, ILogger logger)
        {
            // Create a default HTTP response
            var errorResult = session.CreateExceptionDetails(error);
            var body = session.HttpClient.Response.Encoding.GetBytes(JsonSerializer.Serialize(errorResult));
            var response = new ExceptionResponse(body);

            // Ensure that problem responses are never cached
            response.Headers.AddHeader("Cache-Control", "no-cache, no-store");
            response.Headers.AddHeader("Pragma", "no-cache");
            response.Headers.AddHeader("Expires", "0");

            // Customize the error message
            response.StatusCode = (int)errorResult.Status!;
            response.HttpVersion = session.HttpClient.Request.HttpVersion;
            response.ContentType = "application/problem+json";

            // Add more information to the logger instance
            var details = new Dictionary<string, object>
            {
                ["ErrorId"] = session.ClientConnectionId,
                ["Instance"] = errorResult.Instance!,
            };
            using var scope = logger.BeginScope(details);

            // Write the response and complete the request pipeline
            logger.LogError(error, errorResult.Detail);
            session.Respond(response);
        }

        private static ExceptionDetails CreateExceptionDetails(this SessionEventArgsBase session, Exception error)
        {
            var errorResult = error is IProductException productException
                ? new ExceptionDetails(productException)
                : new ExceptionDetails(new InternalServerException(error.Message));

            errorResult.Instance = session.HttpClient.Request.Url;
            errorResult.Extensions.Add("ErrorId", session.ClientConnectionId);

            if (Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") == "Development")
            {
                errorResult.Extensions.Add("StackTrace", error.StackTrace);
            }

            return errorResult;
        }
    }
}
