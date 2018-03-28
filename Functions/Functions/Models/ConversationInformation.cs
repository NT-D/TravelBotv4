using Microsoft.Bot.Schema;
using Microsoft.WindowsAzure.Storage.Table;

namespace Functions.Models
{
    public class ConversationInformation : TableEntity
    {
        public ConversationReference ConversationReference { get; set; }
        public string MessageFromUser { get; set; }
    }
}
