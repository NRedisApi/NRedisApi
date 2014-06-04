using System;

namespace NRedisApi.Fluent
{
    public class RedisCommandConfigurationException : Exception
    {
        public RedisCommandConfigurationException(string message) : base(message)
        {
        }
    }
}