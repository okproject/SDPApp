using System.Collections.Generic;

namespace SDPApp.Core.Abstraction
{
    public interface ISdpExtractor
    {
        IEnumerable<ExtractedMessage> Extract(IEnumerable<string> rawSdpMessages);
    }
}