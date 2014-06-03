using System;

namespace NRedisApi.Fluent
{
    public class RedisSetupIncompleteException : Exception
    {
        public RedisSetupIncompleteException(string message) : base(message)
        {
        }
    }
}