using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Functions.Models;

namespace Functions.WebhookStartChat
{
    public static class WebhookStartChat
    {
        [FunctionName("WebhookStartChat")]
        [return: Table("ChatPlusInformation", Connection= "HumanHandsoffStorage")]
        public static async Task<ChatPlusInformation> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // Get request body
            var res = await req.Content.ReadAsAsync<ChatPlusInformation>();

            // Set the response to storage
            res.PartitionKey = "ChatPlusInformation";
            res.RowKey = res.visitor.visitor_id;
            return res;
        }
    }
}
