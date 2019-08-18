using System.Collections.Generic;
using System.Threading.Tasks;

namespace SDPApp.Core.Abstraction
{
    public interface ISdpExtractor
    {
        Task<IEnumerable<ExtractedMessage>> Extract(IEnumerable<string> rawSdpMessages);
    }
}