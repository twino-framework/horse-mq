﻿using System.Threading.Tasks;
using Horse.Messaging.Protocol;
using Horse.Messaging.Server.Clients;

namespace Horse.Messaging.Server.Queues.Delivery
{
    public interface IQueueDeliveryHandler
    {
        IHorseQueueManager Manager { get; }
        IDeliveryTracker Tracker { get; }

        /// <summary>
        /// When a client sends a message to the server.
        /// </summary>
        Task<Decision> ReceivedFromProducer(HorseQueue queue, QueueMessage message, MessagingClient sender);

        /// <summary>
        /// Before send the message.
        /// When this method is called, message isn't sent to anyone.
        /// </summary>
        Task<Decision> BeginSend(HorseQueue queue, QueueMessage message);

        /// <summary>
        /// Before sending message to a receiver.
        /// This method is called for each message and each receiver.
        /// This method decides if it is sent.
        /// </summary>
        Task<bool> CanConsumerReceive(HorseQueue queue, QueueMessage message, MessagingClient receiver);

        /// <summary>
        /// After sending message to a receiver.
        /// This method is called for each message and each receiver.
        /// </summary>
        Task<Decision> ConsumerReceiveFailed(HorseQueue queue, MessageDelivery delivery, MessagingClient receiver);

        /// <summary>
        /// Called when a message sending operation is completed.
        /// </summary>
        Task<Decision> EndSend(HorseQueue queue, QueueMessage message);

        /// <summary>
        /// Called when a receiver sends an acknowledge message.
        /// </summary>
        Task<Decision> AcknowledgeReceived(HorseQueue queue, HorseMessage acknowledgeMessage, MessageDelivery delivery, bool success);

        /// <summary>
        /// Called when message requested acknowledge but acknowledge message isn't received in time
        /// </summary>
        Task<Decision> AcknowledgeTimeout(HorseQueue queue, MessageDelivery delivery);
    }
}