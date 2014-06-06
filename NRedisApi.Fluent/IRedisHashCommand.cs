using System.Collections.Generic;

namespace NRedisApi.Fluent
{
    public interface IRedisHashCommand
    {
        IRedisHashCommand SetUrn(string urn);
        IRedisHashCommand<T> AsType<T>();
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
        IRedisHashCommand<T> SetUrn(string urn);
    }
}