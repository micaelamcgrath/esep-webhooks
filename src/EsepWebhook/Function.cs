using System.Text;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using System.Net.Http;
using System.IO;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace EsepWebhook
{
    public class Function
    {
        /// <summary>
        /// This function processes the GitHub webhook to extract the issue URL and sends it to Slack.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public string FunctionHandler(object input, ILambdaContext context)
        {
            // Log the incoming data for debugging purposes
            context.Logger.LogInformation($"FunctionHandler received: {input}");

            // Deserialize the incoming webhook JSON payload
            dynamic json = JsonConvert.DeserializeObject<dynamic>(input.ToString());

            // Extract the URL of the created issue
            string issueUrl = json?.issue?.html_url;

            // If the issue URL is found, send it to Slack
            if (!string.IsNullOrEmpty(issueUrl))
            {
                // Prepare the payload for the Slack message
                string payload = $"{{'text':'Issue Created: {issueUrl}'}}";
                
                // Create an HTTP client to send the request to Slack
                var client = new HttpClient();
                var webRequest = new HttpRequestMessage(HttpMethod.Post, Environment.GetEnvironmentVariable("SLACK_URL"))
                {
                    Content = new StringContent(payload, Encoding.UTF8, "application/json")
                };

                // Send the request to Slack and read the response
                var response = client.Send(webRequest);
                using var reader = new StreamReader(response.Content.ReadAsStream());

                // Return the response from Slack (or an error if something went wrong)
                return reader.ReadToEnd();
            }
            else
            {
                // If the issue URL is not found, log an error and return a message
                context.Logger.LogError("Error: Issue URL not found in the webhook data.");
                return "Error: No issue URL in the webhook data.";
            }
        }
    }
}

