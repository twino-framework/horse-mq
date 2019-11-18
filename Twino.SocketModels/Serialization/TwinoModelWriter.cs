using System.IO;
using Newtonsoft.Json;

namespace Twino.SocketModels.Serialization
{
    /// <summary>
    /// Default model writer for Twino libraries
    /// </summary>
    public class TwinoModelWriter : IModelWriter
    {
        private static TwinoModelWriter _default;

        /// <summary>
        /// Default, singleton TwinoModelWriter class
        /// </summary>
        public static TwinoModelWriter Default
        {
            get
            {
                if (_default == null)
                    _default = new TwinoModelWriter();

                return _default;
            }
        }

        /// <summary>
        /// Creates serialized string message from T model
        /// </summary>
        public string Serialize(ISocketModel model)
        {
            string body;

            if (model is IPerformanceCriticalModel critical)
            {
                LightJsonWriter writer = new LightJsonWriter();
                body = writer.Serialize(critical);
            }
            else
                body = JsonConvert.SerializeObject(model);

            return $"[{model.Type},{body}]";
        }
    }
}