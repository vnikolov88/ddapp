using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace DDApp.DynamicMapping
{
    public class NoThrowExpandoObject : DynamicObject
    {
        private Dictionary<string, object> _properties;

        public NoThrowExpandoObject(Dictionary<string, object> properties)
        {
            _properties = properties ?? throw new ArgumentNullException(nameof(properties));
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
