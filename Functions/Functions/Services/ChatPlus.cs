using System;
using System.Configuration;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Functions.Models;

namespace Functions.Services
{
    public static class ChatPlus
    {
        public static HttpClient httpClient = new HttpClient();
        public static string url = ConfigurationManager.AppSettings["ChatPlusBaseUrl"] + "send";
        public static async Task<bool> SendConnectionRequest()
        {
            // TODO hiroaki-honda Implement logic to send connection request
            return true;
        }

        public static async Task<bool> SendMessage(ChatPlusInformation chatPlusInformation, ConversationInformation conversationInformation)
        {
            var payload = new MessageToAgent()
            {
                to = chatPlusInformation.room.room_id,
                agent = chatPlusInformation.agent.agent_id,
                messages = new Message[1]
                {
                    new Message()
                    {
                        type = "text",
                        text = conversationInformation.MessageFromUser
                    }
                },
                accessToken = ConfigurationManager.AppSettings["ChatPlusAccessToken"],
                siteId = chatPlusInformation.site.site_id
            };
            var res = await httpClient.PostAsync<MessageToAgent>(url, payload, null);
            return res.IsSuccessStatusCode;
        }
    }

    public class MessageToAgent
    {
        public string to { get; set; }
        public int agent { get; set; }
        public Message[] messages { get; set; }
        public string accessToken { get; set; }
        public int siteId { get; set; }
    }

    public class Message
    {
        public string type { get; set; }
        public string text { get; set; }
        public string url { get; set; }
        public string filename { get; set; }
        public Option[] options { get; set; }
    }

    public class Option
    {
        public string type { get; set; }
        public string label { get; set; }
        public string value { get; set; }
    }
}
