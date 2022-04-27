using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace acsahealthCheckResponse
{
    public static class acsa_healthCheck_response
    {
        [FunctionName("acsa_healthCheck_response")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var successBody = JsonConvert.SerializeObject(new
            {
                name = "TaskCompleted",
                status = "successful"
            });

            return new OkObjectResult(successBody);
        }
    }
}
