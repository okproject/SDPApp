using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SDPApp.Core;
using SDPApp.Core.Abstraction;

namespace SDPApp.Infrastructure
{
    public class SdpExtractorSequential : ISdpExtractor
    {
        private readonly IFileWriter _fileWriter;
        private readonly IIPParser _ipParser;
        private readonly IPortParser _portParser;
        private readonly ICodecParser _codecParser;
        private ConcurrentBag<ExtractedMessage> _resultBag = new ConcurrentBag<ExtractedMessage>();
        private ExtractorSettings _extractorSettings;

        public SdpExtractorSequential(IFileWriter fileWriter, IIPParser ipParser, IPortParser portParser,
            ICodecParser codecParser, ExtractorSettings extractorSettings)
        {
            _fileWriter = fileWriter;
            _ipParser = ipParser;
            _portParser = portParser;
            _codecParser = codecParser;
            _extractorSettings = extractorSettings;
        }

        public async Task<IEnumerable<ExtractedMessage>> Extract(IEnumerable<string> rawSdpMessages)
        {
            var sw = new Stopwatch();
            sw.Start();
            var resultMessageList = new List<string>();
            foreach (var message in rawSdpMessages)
            {
                var ip = _ipParser.ParseIp(message);
                var port = _portParser.ParsePort(message);
                var codecs = _codecParser.ParseCodecs(message);

                var extractedMessage = new ExtractedMessage
                {
                    Ip = ip,
                    Port = port,
                    Codecs = codecs
                };
                _resultBag.Add(extractedMessage);
            }
            sw.Stop();
            Console.WriteLine($"Sequential elapsed: {sw.Elapsed.TotalSeconds}");

            resultMessageList = _resultBag.Select(x => x.ToString()).ToList();
            _fileWriter.WriteToFile(_extractorSettings.OutputFileFullPath, resultMessageList);
            return _resultBag.ToList();
        }
    }
}