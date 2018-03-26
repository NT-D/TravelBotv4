﻿using Microsoft.Bot.Connector;
using Underscore.Bot.Models;

namespace Underscore.Bot.MessageRouting
{
    public enum MessageRouterResultType
    {
        NoActionTaken, // No action taken - The result handler should ignore results with this type
        OK, // Action taken, but the result handler should ignore results with this type
        ConnectionRequested,
        ConnectionAlreadyRequested,
        ConnectionRejected,
        Connected,
        Disconnected,
        NoAgentsAvailable,
        NoAggregationChannel,
        FailedToForwardMessage,
        Error // Generic error including e.g. null arguments
    }

    /// <summary>
    /// Represents a result of more complex operations executed by MessageRouterManager (when
    /// boolean just isn't enough).
    /// 
    /// Note that - as this class serves different kind of operations with different kind of
    /// outcomes - some of the properties can be null. The type of the result defines which
    /// properties are meaningful.
    /// </summary>
    public class MessageRouterResult
    {
        public MessageRouterResultType Type
        {
            get;
            set;
        }

        /// <summary>
        /// Activity instance associated with the result.
        /// </summary>        
        public Activity Activity
        {
            get;
            set;
        }

        /// <summary>
        /// A valid ConversationResourceResponse of the newly created direct conversation
        /// (between the bot [who will relay messages] and the conversation owner),
        /// if the connection was added and a conversation created successfully
        /// (MessageRouterResultType is Connected).
        /// </summary>
        public ConversationResourceResponse ConversationResourceResponse
        {
            get;
            set;
        }

        public Party ConversationOwnerParty
        {
            get;
            set;
        }

        public Party ConversationClientParty
        {
            get;
            set;
        }

        public string ErrorMessage
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MessageRouterResult()
        {
            Type = MessageRouterResultType.NoActionTaken;
            ErrorMessage = string.Empty;
        }

        public override string ToString()
        {
            return $"[{Type}; {ConversationResourceResponse}; {ConversationOwnerParty}; {ConversationClientParty}; {ErrorMessage}]";
        }
    }
}
