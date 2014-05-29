using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NRedisApi.Fluent
{
    public class RedisOperation
    {
        protected string _urn;
        protected RedisDataStructure _redisDataStructure;

        public RedisOperation()
        {
            _urn = string.Empty;
            _redisDataStructure = RedisDataStructure.Unknown;
        }

        public RedisOperation Urn(string urn)
        {
            _urn = urn;
            return this;
        }

        public RedisOperation Hash()
        {
            _redisDataStructure = RedisDataStructure.Hash;
            return this;
        }

        public RedisOperation String()
        {
            _redisDataStructure = RedisDataStructure.String;
            return this;
        }

        public RedisOperation List()
        {
            _redisDataStructure = RedisDataStructure.List;
            return this;
        }

        public RedisOperation Set()
        {
            _redisDataStructure = RedisDataStructure.Set;
            return this;
        }

        public RedisOperation SortedSet()
        {
            _redisDataStructure = RedisDataStructure.SortedSet;
            return this;
        }

        public RedisOperation<T> AsType<T>()
        {
            return new RedisOperation<T>(_urn, _redisDataStructure);
        }
    }

    public class RedisOperation<T> : RedisOperation
    {
        public RedisOperation(string urn, RedisDataStructure redisDataStructure)
        {
            _urn = urn;
            _redisDataStructure = redisDataStructure;
        }
    }
}
