using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Bot.Schema;


namespace TravelBotv4.Models
{
    public class ConversationInformation : TableEntity
    {
        public string ConversationReference { get; set; }
        public string MessageFromUser { get; set; }
    }
}
