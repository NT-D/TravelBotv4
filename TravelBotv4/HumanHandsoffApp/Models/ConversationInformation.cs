using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace HumanHandsoffApp.Models
{
    public class ConversationInformation : TableEntity
    {
        public string ConversationReference { get; set; }
        public string MessageFromUser { get; set; }
    }
}
