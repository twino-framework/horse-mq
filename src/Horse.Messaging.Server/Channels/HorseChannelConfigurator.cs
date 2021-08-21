using Horse.Messaging.Server.Containers;

namespace Horse.Messaging.Server.Channels
{
    /// <summary>
    /// Horse channel configurator
    /// </summary>
    public class HorseChannelConfigurator
    {
        /// <summary>
        /// Default channel options
        /// </summary>
        public HorseChannelOptions Options => _rider.Channel.Options;

        /// <summary>
        /// Event handlers to track channel events
        /// </summary>
        public ArrayContainer<IChannelEventHandler> EventHandlers => _rider.Channel.EventHandlers;

        /// <summary>
        /// Channel authenticators
        /// </summary>
        public ArrayContainer<IChannelAuthorization> Authenticators => _rider.Channel.Authenticators;

        private readonly HorseRider _rider;

        internal HorseChannelConfigurator(HorseRider rider)
        {
            _rider = rider;
        }
    }
}