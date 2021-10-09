using System.Collections.Generic;
using System.Threading.Tasks;
using Horse.Messaging.Protocol;

namespace Horse.Messaging.Server.Queues.Store
{
    /// <summary>
    /// Queue message store implementation stores queue messages.
    /// </summary>
    public interface IQueueMessageStore
    {
        IHorseQueueManager Manager { get; }
        IMessageTimeoutTracker TimeoutTracker { get; }
        
        bool IsEmpty { get; }

        /// <summary>
        /// Returns count of all stored messages
        /// </summary>
        /// <returns></returns>
        int Count();

        /// <summary>
        /// Puts a message into message store 
        /// </summary>
        void Put(QueueMessage message);

        /// <summary>
        /// Gets next message from store
        /// </summary>
        QueueMessage ReadFirst();

        /// <summary>
        /// Gets next message from store
        /// </summary>
        QueueMessage ConsumeFirst();
        
        /// <summary>
        /// Gets next message from store
        /// </summary>
        List<QueueMessage> ConsumeMultiple(int count);
        
        /// <summary>
        /// Gets all messages.
        /// That method returns the messages without thread safe
        /// </summary>
        IEnumerable<QueueMessage> GetUnsafe();
        
        /// <summary>
        /// Finds and removes message from store
        /// </summary>
        bool Remove(string messageId);

        /// <summary>
        /// Finds and removes message from store
        /// </summary>
        void Remove(HorseMessage message);

        /// <summary>
        /// Finds and removes message from store
        /// </summary>
        void Remove(QueueMessage message);

        /// <summary>
        /// Clears all messages from store
        /// </summary>
        Task Clear();

        /// <summary>
        /// Destroys all messages
        /// </summary>
        /// <returns></returns>
        Task Destroy();
    }
}