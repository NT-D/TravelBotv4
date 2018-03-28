#r "Newtonsoft.Json"
using System.Net;
using System.Linq;
using System.Collections.Specialized;
using Newtonsoft.Json;

public static async Task<ChatPlusInformation> Run(HttpRequestMessage req, ICollector<ChatPlusInformation> outputTable, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");

    var data = await req.Content.ReadAsFormDataAsync();
    var dict = data.AllKeys.ToDictionary(p => p, p => data[p]);
    var json = JsonConvert.SerializeObject(dict);
    var res = JsonConvert.DeserializeObject<ChatPlusInformation>(json);

    log.Info($"******** Response of webhook which is triggered when chat start ********");
    log.Info($"{JsonConvert.SerializeObject(res)}");
    log.Info($"************************************************************************");

    res.PartitionKey = "ChatPlusInformation";
    res.RowKey = res.visitor.visitor_id;

    // Save ChatPlus information to table storage
    outputTable.Add(res);

    // Send event messagte to Bot    
    return res;
}

public class ChatPlusInformation
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
    public string email { get; set; }
    public string company_name { get; set; }
    public string tel { get; set; }
    public string score { get; set; }
    public string ip_addr { get; set; }
    public string visit_count { get; set; }
    public Page page { get; set; }
    public string useragent { get; set; }
}
