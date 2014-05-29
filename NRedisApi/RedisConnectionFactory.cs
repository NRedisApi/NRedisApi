﻿using System.Configuration;
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

        public RedisConnectionFactory(ConnectionMultiplexer conn)
        {
            _conn = conn;
            if (!_conn.IsConnected)
                _conn = ConnectionMultiplexer.Connect(ConfigurationManager.AppSettings["RedisHost"]); //TO DO - add other config options
        }

        public RedisConnection GetConnection()
        {
            return new RedisConnection(_conn.GetDatabase());
        }
    }
}
