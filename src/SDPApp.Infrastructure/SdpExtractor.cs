using System.Collections.Generic;
using System.Threading.Tasks;
using SDPApp.Core;
using SDPApp.Core.Abstraction;

namespace SDPApp.Infrastructure
{
    public class SdpExtractor:ISdpExtractor
    {
        private IIPParser _ipParser;
        private IPortParser _portParser;
        private ICodecParser _codecParser;

        public SdpExtractor(IIPParser ipParser, IPortParser portParser, ICodecParser codecParser)
        {
            _ipParser = ipParser;
            _portParser = portParser;
            _codecParser = codecParser;
        }

        public async Task<IEnumerable<ExtractedMessage>> Extract(IEnumerable<string> rawSdpMessages)
        {
            //Configurations
            
            //Transfromers
            return  new List<ExtractedMessage>();
        }
    }
}