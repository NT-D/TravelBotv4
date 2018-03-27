using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace Functions.WebhookStartChat
{
    public static class WebhookStartChat
    {
        [FunctionName("WebhookStartChat")]
        [return: Table("WebhookStartChatResponse", Connection= "HumanHandsoffStorage")]
        public static async Task<WebhookStartChatResponse> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // Get request body
            var res = await req.Content.ReadAsAsync<WebhookStartChatResponse>();

            // Set the response to storage
            res.PartitionKey = "WebhookStartChatResponse";
            res.RowKey = res.visitor.visitor_id;
            return res;
        }
    }

    public class WebhookStartChatResponse
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string type { get; set; }
        public string timestamp { get; set; }
        public Site site { get; set; }
        public Room room { get; set; }
        public Agent agent { get; set; }
        public Visitor visitor { get; set; }

    }

    public class Site
    {
        public string site_id { get; set; }
        public string site_name { get; set; }
    }

    public class Room
    {
        public string room_id { get; set; }
        public string status { get; set; }
    }

    public class Agent
    {
        public string agent_id { get; set; }
        public string agent_name { get; set; }
        public string agent_display_name { get; set; }
    }

    public class Page
    {
        public string page_url { get; set; }
        public string page_title { get; set; }
    }

    public class Visitor
    {
        public string visitor_id { get; set; }
        public string user_name { get; set; }
        public string email { get; set; }
        public string company_name { get; set; }
        public string tel { get; set; }
        public string score { get; set; }
        public string ip_addr { get; set; }
        public string visit_count { get; set; }
        public Page page { get; set; }
        public string useragent { get; set; }
    }
}
