using System.Collections.Generic;

namespace NRedisApi.Fluent
{
    public interface IRedisHashCommand
    {
        IRedisHashCommand Urn(string urn);
        IRedisHashCommand<T> As<T>();
    }

    public interface IRedisHashCommand<T>
    {
        T Get();
        void Set(T value);
        void Remove();
        IRedisHashCommand<T> UniqueIdFieldName(string fieldName);
        IRedisHashCommand<T> UniqueIdFieldValues(IDictionary<string, string> uidNameValuePairs);
        IRedisHashCommand<T> Urn(string urn);
    }
}