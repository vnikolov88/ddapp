using Convert = System.Convert;
using Encoding = System.Text.Encoding;

namespace DDApp.Extensions
{
    public static class StringExtensions
    {
        public static string OnDevice(this string url) => $"/ondevice/{url}";

        public static string OnDeviceCall(this string number) => $"/ondevice/tel:{number}";

        public static string OnDeviceFax(this string fax) => $"/ondevice/fax:{fax}";

        public static string OnDeviceMail(this string email) => $"/ondevice/mailto:{email}";

        public static string ProxyRemoteUrl(this string url) => url.StartsWith('/') ? url : $"/proxyremote/{url.Base64Encode()}"; // Note: we dont proxy local requests

        public static string Base64Encode(this string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(this string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
