using Newtonsoft.Json;
using System;

namespace DDApp.Extensions
{
    public static class GUIDExtensions
    {
        public static ulong GetGUID(this string self)
        {
            if (string.IsNullOrWhiteSpace(self)) return WithGUID.GUID_Empty;

            ulong hash = 5381;

            foreach (var c in self)
                hash = ((hash << 5) + hash) + c; /* hash * 33 + c */

            return hash;
        }

        public static ulong GetGUID(this uint self)
        {
            unchecked
            {
                ulong hash = 5381;

                return hash * self - 5;
            }
        }

        public static ulong GetGUID(this float self)
        {
            return GetGUID((uint) self);
        }

        public static ulong MixGUID(this ulong self, ulong other)
        {
            unchecked
            {
                return self * other - 5;
            }
        }

        public static ulong GetGUID(this Type self) => $"{self.Assembly.FullName}:{self.FullName}".GetGUID();
    }

    public interface IGUID
    {
        ulong GUID { get; }
    }

    public abstract class WithGUID : IGUID
    {
        public const ulong GUID_Empty = 0;

        private ulong _guid = GUID_Empty;
        [JsonIgnore]
        public ulong GUID => _guid != GUID_Empty ? _guid : _guid = getGUID();

        protected abstract ulong getGUID();
    }
}
