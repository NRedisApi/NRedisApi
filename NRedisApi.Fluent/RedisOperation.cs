﻿using System;
using System.Collections.Generic;
using StackExchange.Redis;

namespace NRedisApi.Fluent
{
    public class RedisOperation
    {
        protected string _urn;
        protected RedisDataStructure _redisDataStructure;
        private readonly IDatabase _redis;

        public RedisOperation(IDatabase redis)
        {
            _redis = redis;
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

        public virtual RedisOperation<T> AsType<T>()
        {
            return new RedisOperation<T>(_urn, _redisDataStructure, _redis);
        }
    }

    public class RedisOperation<T> : RedisOperation
    {
        private readonly IDatabase _redis;

        internal RedisOperation(string urn, RedisDataStructure redisDataStructure, IDatabase redis)
            : base(redis)
        {
            _redis = redis;
            _urn = urn;
            _redisDataStructure = redisDataStructure;
        }

        public IEnumerable<T> GetCollection()
        {
            throw new NotImplementedException();
        }

        public T Get()
        {
            throw new NotImplementedException();
        }

        public override RedisOperation<U> AsType<U>()
        {
            throw new TypeAlreadySpecifiedException("Type already specified. You cannot set two <T> return types");
        }
    }

    public class TypeAlreadySpecifiedException : Exception
    {
        public TypeAlreadySpecifiedException(string message) : base(message)
        {

        }
    }
}
