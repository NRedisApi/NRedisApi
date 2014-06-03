using System;

namespace NRedisApi
{
    public interface IRedisConnection
    {
        /// <summary>
        /// Gets Redis String (data structure) deserialised from JSON to T by URN key
        /// </summary>
        /// <typeparam name="T">Type to deserialise to</typeparam>
        /// <param name="key">Redis URN</param>
        /// <returns></returns>
        T GetValue<T>(string key);

        /// <summary>
        /// Sets Redis String (data structure) of type T, serialised to JSON
        /// </summary>
        /// <typeparam name="T">Type of object being stored</typeparam>
        /// <param name="key">Redis URN</param>
        /// <param name="value">instance of T to be serialised and set as Redis string</param>
        void Save<T>(string key, T value);

        /// <summary>
        /// Sets Redis Hash (data structure) item of type T, serialised to JSON, to create a collection of serialised .Net objects using Hash-Field name to identify objects and value
        /// to store JSON representation of object. 
        /// Creates Redis Hash-Field name using string array of unique identifier fields' values (similar to composite primary key in SQL) via Reflection.
        /// Its highly weakly-typed nature and potential for confusion and/or typos with array of strings is in part what Fluent API approach is intended to mitigate.
        /// </summary>
        /// <typeparam name="T">Type of object being stored</typeparam>
        /// <param name="key">Redis Hash URN</param>
        /// <param name="value">instance of T to be serialised and set as Redis Hash-Field value</param>
        /// <param name="uniqueIdentifierFieldNames">string array of field names the values of which will be used to generate Hash-Field names to ID each object in collection</param>
        void SaveToHash<T>(string key, T value, string[] uniqueIdentifierFieldNames);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="uniqueIdentifierFieldNames"></param>
        /// <param name="uniqueIdentifierFieldNamesAndValues"></param>
        /// <returns></returns>
        T FindFromHash<T>(string key, string[] uniqueIdentifierFieldNames, Tuple<string, string>[] uniqueIdentifierFieldNamesAndValues);
    }
}