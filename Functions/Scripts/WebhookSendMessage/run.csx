#r "Newtonsoft.Json"
using System.Net;
using System.Collections.Specialized;
using System.Linq;
using Newtonsoft.Json;

public static async Task<WebhookSendMessageResponse> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");

    // Get request body
    var data = await req.Content.ReadAsFormDataAsync();
    var dict = data.AllKeys.ToDictionary(p => p, p => data[p]);
    var json = JsonConvert.SerializeObject(dict);
    var webhookResponse = JsonConvert.DeserializeObject<WebhookSendMessageResponse>(json);

    log.Info($"******** Response of webhook which is triggered when message sent from agent ********");
    log.Info($"{json}");
    log.Info($"*************************************************************************************");

    // Send event type message to Bot
    return webhookResponse;
}

public class WebhookSendMessageResponse
{
    public string type { get; set; }
    public string timestamp { get; set; }
    public Site site { get; set; }
    public Room room { get; set; }
    public Agent agent { get; set; }
    public Visitor visitor { get; set; }
    public Message message { get; set; }
}

public class Site
{
    public int site_id { get; set; }
    public string site_name { get; set; }
}

public class Room
{
    public string room_id { get; set; }
    public string status { get; set; }
}

public class Agent
{
    public int agent_id { get; set; }
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
    public string external_key { get; set; }
    public Page page { get; set; }
    public string useragent { get; set; }
}

public class Message
{
    public string id { get; set; }
    public string type { get; set; }
    public string text { get; set; }
}
