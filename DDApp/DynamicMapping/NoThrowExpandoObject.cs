using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace DDApp.DynamicMapping
{
    public class NoThrowExpandoObject : DynamicObject
    {
        private Dictionary<string, object> _properties = new Dictionary<string, object>();

        public NoThrowExpandoObject(IDictionary<string, StringValues> properties)
        {
            foreach (var kvp in properties)
            {
                _properties.Add(kvp.Key, kvp.Value);
            }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (!_properties.ContainsKey(binder.Name))
            {
                result = GetDefault(binder.ReturnType);
                return true;
            }

            return _properties.TryGetValue(binder.Name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            this._properties[binder.Name] = value;
            return true;
        }

        private static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
    }
}
