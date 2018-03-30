namespace HumanHandoffApp.Models
{
    //public class ChatPlusWebhook
    //{

    //}
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
        public string email { get; set; }
        public string company_name { get; set; }
        public string tel { get; set; }
        public string score { get; set; }
        public string ip_addr { get; set; }
        public string visit_count { get; set; }
        public Page page { get; set; }
        public string useragent { get; set; }
    }

    public class Message
    {
        public string id { get; set; }
        public string type { get; set; }
        public string text { get; set; }
    }
}
