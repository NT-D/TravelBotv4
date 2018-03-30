using Microsoft.WindowsAzure.Storage.Table;


namespace HumanHandoffApp.Models
{
    public class ConversationInformation : TableEntity
    {
        public string ConversationReference { get; set; }
        public string MessageFromUser { get; set; }
    }
}
