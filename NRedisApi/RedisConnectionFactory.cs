using Newtonsoft.Json;
using NRedisApi.Fluent;
using StackExchange.Redis;

namespace NRedisApi
{
    /// <summary>
    /// Connection Factory to hold singleton instance of StackExchange.Redis.ConnectionMultiplexer -> should be configured as such (factory and ConnectionMultiplexer both being singletons 
    /// in IoC Container, as per StructureMap examples provided.
    /// </summary>
    public class RedisConnectionFactory : IRedisConnectionFactory
    {
        private readonly ConnectionMultiplexer _conn;
        private ConfigurationOptions _configurationOptions;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public RedisConnectionFactory(ConnectionMultiplexer conn, ConfigurationOptions configuration, JsonSerializerSettings jsonSerializerSettings)
        {
            _configurationOptions = configuration;
            _jsonSerializerSettings = jsonSerializerSettings;
            _conn = conn;
            if (!_conn.IsConnected)
                _conn = ConnectionMultiplexer.Connect(_configurationOptions); //TO DO - add other config options
        }

        public IRedisCommand GetConnection()
        {
            return new RedisCommand(_conn.GetDatabase(), _jsonSerializerSettings);
        }

        //public void SetConfiguration(ConfigurationOptions configuration)
        //{
        //    _configurationOptions = configuration;
        //}


    }
}
