using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Underscore.Bot.MessageRouting;

namespace TravelBotv4.MessageRouting
{
    public class MessageRouterResultHandler
    {
        private MessageRouterManager _messageRouterManager;

        public MessageRouterResultHandler(MessageRouterManager messageRouterManager)
        {
            _messageRouterManager = messageRouterManager
                ?? throw new ArgumentNullException(
                    $"The message router manager ({nameof(messageRouterManager)}) cannot be null");
        }
    }
}
