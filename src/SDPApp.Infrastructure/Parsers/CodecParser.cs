using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SDPApp.Core.Abstraction;

namespace SDPApp.Infrastructure.Parsers
{
    public class CodecParser : ICodecParser
    {
        private const string _codecPattern = @"(?<=a=rtpmap:[0-9] ).[^/]*";

        public IEnumerable<string> ParsePort(string rawSdpMessage)
        {
            var codecResult = Regex.Matches(rawSdpMessage, _codecPattern);
            return codecResult?.OfType<Match>()?.Select(cdc => cdc.Value)?.ToArray();
        }
    }
}