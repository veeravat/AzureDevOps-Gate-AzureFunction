using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace healthCheck
{
    public static class acsa_healthCheck
    {
        [FunctionName("acsa_healthCheck")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var url = req.Headers["PlanUrl"];
            var projectId = req.Headers["ProjectId"];
            var hubName = req.Headers["HubName"];
            var planId = req.Headers["PlanId"];
            var jobId = req.Headers["JobId"];
            var timelineId = req.Headers["TimelineId"];
            var taskInstanceId = req.Headers["TaskinstanceId"];
            var authToken = req.Headers["AuthToken"];

            var callbackUrl = $"{url}/{projectId}/_apis/distributedtask/hubs/{hubName}/plans/{planId}/events?api-version=2.0-preview.1";
            log.LogInformation($"CallBackUrl = {callbackUrl}");

            // Check something
            var successBody = JsonConvert.SerializeObject(new
            {
                name = "TaskCompleted",
                taskId = taskInstanceId.ToString(),
                jobId = jobId.ToString(),
                result = "succeeded"
            });

            // the following call does not block
            Task.Run(() =>
            {
                Thread.Sleep(20000); // simulate long running work
                PostEvent(callbackUrl, successBody, authToken, log);
            });

            return new OkObjectResult("Long-running job succesfully scheduled!");
        }
        public static void PostEvent(String callbackUrl, String body, String authToken, ILogger log)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                var requestContent = new StringContent(body, Encoding.UTF8, "application/json");
                var response = client.PostAsync(new Uri(callbackUrl), requestContent).Result;
                var responseContent = response.Content.ReadAsStringAsync().Result;
                log.LogInformation(response.StatusCode.ToString());
                log.LogInformation(responseContent);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
            }
        }
    }
}
