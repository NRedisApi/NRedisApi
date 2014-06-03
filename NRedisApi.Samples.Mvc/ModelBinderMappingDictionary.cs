using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace NRedisApi.Samples.Mvc
{
    public class ModelBinderMappingDictionary : Dictionary<Type, Type>
    {
        public void Add<TKey, TModelBinder>()
            where TKey : class
            where TModelBinder : IModelBinder
        {
            Add(typeof(TKey), typeof(TModelBinder));
        }
    }
}