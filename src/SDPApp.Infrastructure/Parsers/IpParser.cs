using System.Text.RegularExpressions;
using SDPApp.Core.Abstraction;

namespace SDPApp.Infrastructure.Parsers
{
    public class IpParser:IIPParser
    {
        private const string _ipPattern= @"(?<=c=IN IP4 ).*";
        public string ParseIp(string rawSdpMessage)
        {
            if (string.IsNullOrEmpty(rawSdpMessage)) return "";
            var ip = Regex.Match( rawSdpMessage, _ipPattern);
            return ip?.Value ?? "";
        }
    }
}