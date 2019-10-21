﻿using System;
using System.Collections.Generic;
using System.Timers;

namespace Twino.Client.Connectors
{
    /// <summary>
    /// Failed message descriptor
    /// </summary>
    internal class FailedMessage
    {
        /// <summary>
        /// Generated message data for websocket procotol
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Message creation date (UTC)
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// True if the message sent successfully
        /// </summary>
        public bool Sent { get; set; }

        /// <summary>
        /// Starts with 1, tells us how many times the send operation is failed.
        /// </summary>
        public int TryCount { get; set; }
    }

    /// <summary>
    /// Derives from StickyConnector.
    /// All features of sticky connector are included.
    /// In addition, when Send method is called,
    /// tries to send the message. If there is no active connection,
    /// it keeps the message and sends when connected.
    /// Each message has it's own maximum try count and expire time.
    /// </summary>
    public class AbsoluteConnector : StickyConnector
    {
        #region Properties

        /// <summary>
        /// Expiration duration for failed messages
        /// </summary>
        public TimeSpan MessageExpiration { get; set; }

        /// <summary>
        /// Maximum re-send try count for failed messages
        /// </summary>
        public int MaximumTryCount { get; set; }

        /// <summary>
        /// Failed messages
        /// </summary>
        private readonly List<FailedMessage> _failedMessages;

        /// <summary>
        /// Failed messages cleanup timer
        /// </summary>
        private readonly Timer _messageTimer;

        /// <summary>
        /// True when failed message processing is started until it finished.
        /// This field is created to avoid multiple processing at same time.
        /// </summary>
        private bool _messagesProcessing;

        #endregion

        public AbsoluteConnector(TimeSpan reconnectInterval) : base(reconnectInterval)
        {
            MessageExpiration = TimeSpan.Zero;
            MaximumTryCount = 0;
            _failedMessages = new List<FailedMessage>();

            Connected += AbsoluteConnector_Connected;

            _messageTimer = new Timer(500);
            _messageTimer.Elapsed += MessageTimerElapsed;
            _messageTimer.AutoReset = true;
            _messageTimer.Start();
        }

        /// <summary>
        /// With this timer, failed messages is proceed
        /// </summary>
        private void MessageTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (_messagesProcessing || _failedMessages.Count == 0)
                return;

            TwinoClient client = GetClient();
            if (client == null || !client.IsConnected)
                return;

            if (_messagesProcessing)
                return;

            ProcessFailedMessages();
        }

        /// <summary>
        /// Sends the message to the server.
        /// If the operation is failed, message will be saved for sending after reconnect.
        /// </summary>
        public override bool Send(byte[] preparedData)
        {
            TwinoClient client = GetClient();
            if (client == null || !client.IsConnected)
            {
                AddFailedMessage(preparedData);
                return false;
            }

            bool sent = client.Send(preparedData);

            if (!sent)
                AddFailedMessage(preparedData);

            return sent;
        }

        /// <summary>
        /// After a message send operation is failed,
        /// Adds the failed message to the failed messages list
        /// </summary>
        private void AddFailedMessage(byte[] preparedData)
        {
            FailedMessage message = new FailedMessage
                                    {
                                        Created = DateTime.UtcNow,
                                        Data = preparedData,
                                        Sent = false,
                                        TryCount = 1
                                    };

            lock (_failedMessages)
                _failedMessages.Add(message);
        }

        /// <summary>
        /// Fired when connection is establies.
        /// </summary>
        private void AbsoluteConnector_Connected(TwinoClient client)
        {
            ProcessFailedMessages();
        }

        protected override void WriteError(TwinoClient client, byte[] data)
        {
            AddFailedMessage(data);
        }

        /// <summary>
        /// Checks all failed messages.
        /// Tries to send the messages.
        /// Removes expired or maximum try exceeded messages.
        /// </summary>
        private void ProcessFailedMessages()
        {
            _messagesProcessing = true;
            List<FailedMessage> removing = new List<FailedMessage>();

            lock (_failedMessages)
            {
                foreach (FailedMessage fm in _failedMessages)
                {
                    if (fm.Sent)
                    {
                        removing.Add(fm);
                        continue;
                    }

                    if (MaximumTryCount > 0 && MaximumTryCount >= fm.TryCount)
                    {
                        removing.Add(fm);
                        continue;
                    }

                    DateTime expiration = fm.Created + MessageExpiration;
                    if (MessageExpiration > TimeSpan.Zero && expiration >= DateTime.UtcNow)
                    {
                        removing.Add(fm);
                        continue;
                    }

                    try
                    {
                        fm.TryCount++;
                        TwinoClient _client = GetClient();

                        if (_client != null && _client.IsConnected)
                            fm.Sent = _client.Send(fm.Data);
                    }
                    catch
                    {
                        fm.Sent = false;
                    }
                }

                if (removing.Count > 0)
                {
                    foreach (FailedMessage remove in removing)
                        _failedMessages.Remove(remove);
                }
            }

            _messagesProcessing = false;
        }
    }
}