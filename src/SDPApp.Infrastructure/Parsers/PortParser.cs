using System.Text.RegularExpressions;
using SDPApp.Core.Abstraction;

namespace SDPApp.Infrastructure.Parsers
{
    public class PortParser:IPortParser
    {
        private const string _portPattern= @"(?<=m=audio )[0-9]*";
        public string ParsePort(string rawSdpMessage)
        {
            if (string.IsNullOrEmpty(rawSdpMessage)) return "";
            var ptrn = @"(?<=m=audio )[0-9]*";
            var port = Regex.Match(rawSdpMessage, _portPattern);
            return port?.Value ?? "";
        }
    }
}