using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Core.Extensions;

namespace TravelBotv4.Models
{
    public class ConnectionState : StoreItem
    {
        public bool IsConnectedToAgent { get; set; } = false;
    }
}
