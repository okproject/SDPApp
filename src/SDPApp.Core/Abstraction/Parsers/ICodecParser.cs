using System.Collections.Generic;

namespace SDPApp.Core.Abstraction
{
    public interface ICodecParser
    {
        IEnumerable<string> ParsePort(string rawSdpMessage);
    }
}