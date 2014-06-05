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
        IEnumerable<T> GetAll(); 
        void Set(T value);
        void Set(IEnumerable<T> values);
        void Remove();
        long Count();

        IRedisHashCommand<T> UniqueIdFieldName(string fieldName);
        IRedisHashCommand<T> UniqueIdFieldValues(IDictionary<string, string> uidNameValuePairs);
        IRedisHashCommand<T> Urn(string urn);
    }
}