using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Functions.Models;
using Functions.Utils;

namespace Functions.WebhookStartChat
{
    public static class WebhookStartChat
    {
        [FunctionName("WebhookStartChat")]
        [return: Table("ChatPlusInformation", Connection= "HumanHandsoffStorage")]
        public static async Task<ChatPlusInformation> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            var data = await req.Content.ReadAsFormDataAsync();
            var res = Util.NameValueCollection2Object<ChatPlusInformation>(data);

            log.Info($"******** Response of webhook which is triggered when chat start ********");
            log.Info($"{JsonConvert.SerializeObject(res)}");
            log.Info($"************************************************************************");

            res.PartitionKey = "ChatPlusInformation";
            res.RowKey = res.visitor.visitor_id;
            return res;
        }
    }
}
