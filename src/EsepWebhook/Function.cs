using System;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json.Linq;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace EsepWebhook
{
    public class Function
    {
        public string FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
        {
            // Log the raw incoming event for debugging
            context.Logger.LogLine($"Received event: {input.Body}");

            // Parse the JSON payload from GitHub webhook
            var jsonPayload = JObject.Parse(input.Body);

            // Extract the HTML URL of the issue from the payload
            var issueUrl = jsonPayload["issue"]?["html_url"]?.ToString();

            // Check if the URL exists
            if (!string.IsNullOrEmpty(issueUrl))
            {
                context.Logger.LogLine($"Issue URL: {issueUrl}");
                return $"Successfully processed webhook. Issue URL: {issueUrl}";
            }
            else
            {
                context.Logger.LogLine("Issue URL not found in the payload.");
                return "Error: Issue URL not found.";
            }
        }
    }
}


