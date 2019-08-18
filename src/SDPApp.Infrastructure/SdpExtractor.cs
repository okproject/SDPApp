using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using SDPApp.Core;
using SDPApp.Core.Abstraction;

namespace SDPApp.Infrastructure
{
    public class SdpExtractor : ISdpExtractor
    {
        private readonly IIPParser _ipParser;
        private readonly IPortParser _portParser;
        private readonly ICodecParser _codecParser;

        private ExtractorSettings
            _extractorSettings; //Parallelism  and output settings. If output specified, then result will be written related file.

        private ConcurrentBag<ExtractedMessage> resultBag = new ConcurrentBag<ExtractedMessage>();
        private ExecutionDataflowBlockOptions _executionDataflowBlockOptions;
        private DataflowLinkOptions _dataflowLinkOptions; //To propagate completion of blocks to their waiters

        public SdpExtractor(IIPParser ipParser, IPortParser portParser, ICodecParser codecParser,
            ExtractorSettings extractorSettings)
        {
            _ipParser = ipParser;
            _portParser = portParser;
            _codecParser = codecParser;
            _extractorSettings = extractorSettings;
            if (extractorSettings == null)
                _extractorSettings = new ExtractorSettings {MaxDegreeOfParallelism = Environment.ProcessorCount};
            _executionDataflowBlockOptions = GetExecutionDataFlowOptions();
            _dataflowLinkOptions = new DataflowLinkOptions
            {
                PropagateCompletion = true
            };
        }

        public async Task<IEnumerable<ExtractedMessage>> Extract(IEnumerable<string> rawSdpMessages)
        {
            //Transformers
            var broadCast = GetBroadcastBlock();
            var joinBlock = GetJoinBlock();
            var finalizerActionBlock = GetFinalizerActionBlock();
            var ipTransformerBlock = GetIpTransformBlock();
            var portTransformerBlock = GetPortTransformBlock();
            var codecsTransformerBlock = GetCodecTransformBlock();

            //Logic
            broadCast.LinkTo(ipTransformerBlock, _dataflowLinkOptions);
            broadCast.LinkTo(portTransformerBlock, _dataflowLinkOptions);
            broadCast.LinkTo(codecsTransformerBlock, _dataflowLinkOptions);
            ipTransformerBlock.LinkTo(joinBlock.Target1, _dataflowLinkOptions);
            portTransformerBlock.LinkTo(joinBlock.Target2, _dataflowLinkOptions);
            codecsTransformerBlock.LinkTo(joinBlock.Target3, _dataflowLinkOptions);
            joinBlock.LinkTo(finalizerActionBlock, _dataflowLinkOptions);

            long counter = 0;

            var sw = new Stopwatch();
            sw.Start();

            //TODO: datasource hould be a stream. it is possible to improve that part via stream reader process; because may be data is too big and memory may be not enough for that
            foreach (var sdpMessageRawStr in rawSdpMessages)
            {
                counter++;

                await broadCast.SendAsync(sdpMessageRawStr).ContinueWith(x =>
                {
                    if (!x.Result) Console.WriteLine($"{counter} rejected");
                });
            }


            broadCast.Complete();
            await finalizerActionBlock.Completion;
            await broadCast.Completion.ContinueWith(c =>
            {
                sw.Stop();
                Console.WriteLine($"Bag count {resultBag.Count}");
                Console.WriteLine($"Total elapsed time in seconds: {sw.Elapsed.TotalSeconds}");
                Console.WriteLine($"Extracted message count: {resultBag.Count().ToString()}");
            });

            var resultMessageList = resultBag.Select(x => x.ToString()).ToList();
            if (string.IsNullOrEmpty(_extractorSettings.OutputFileFullPath) &&
                Directory.Exists(_extractorSettings.OutputFileFullPath))
            {
                File.WriteAllLines(_extractorSettings.OutputFileFullPath, resultMessageList, Encoding.UTF8);
            }

            return resultBag;
        }

        private ExecutionDataflowBlockOptions GetExecutionDataFlowOptions()
        {
            var edfo = new ExecutionDataflowBlockOptions()
                {SingleProducerConstrained = true, MaxDegreeOfParallelism = _extractorSettings.MaxDegreeOfParallelism};
            return edfo;
        }

        private DataflowLinkOptions GetDataFlowLinkOptions()
        {
            var dataFlowLinkOptions = new DataflowLinkOptions
            {
                PropagateCompletion = true
            };
            return dataFlowLinkOptions;
        }

        #region Transformers

        private TransformBlock<string, string> GetIpTransformBlock()
        {
            var ipTransformer = new TransformBlock<string, string>(x => { return _ipParser.ParseIp(x); },
                new ExecutionDataflowBlockOptions()
                {
                    MaxDegreeOfParallelism = _extractorSettings.MaxDegreeOfParallelism, SingleProducerConstrained = true
                });
            return ipTransformer;
        }

        private TransformBlock<string, string> GetPortTransformBlock()
        {
            var portTransformBlock = new TransformBlock<string, string>(x => { return _portParser.ParsePort(x); },
                new ExecutionDataflowBlockOptions()
                {
                    MaxDegreeOfParallelism = _extractorSettings.MaxDegreeOfParallelism, SingleProducerConstrained = true
                });
            return portTransformBlock;
        }

        private TransformBlock<string, string[]> GetCodecTransformBlock()
        {
            var codecTransformBlock = new TransformBlock<string, string[]>(
                x => { return _codecParser.ParsePort(x).ToArray(); },
                new ExecutionDataflowBlockOptions()
                {
                    MaxDegreeOfParallelism = _extractorSettings.MaxDegreeOfParallelism, SingleProducerConstrained = true
                });
            return codecTransformBlock;
        }

        private ActionBlock<Tuple<string, string, string[]>> GetFinalizerActionBlock()
        {
            var finalizerActionBlock = new ActionBlock<Tuple<string, string, string[]>>(fab =>
                {
                    var (ip, port, codecs) = fab;
                    var extractedMessage = new ExtractedMessage()
                    {
                        Ip = ip, Codecs = codecs, Port = port
                    };
                    resultBag.Add(extractedMessage);
                },
                new ExecutionDataflowBlockOptions()
                {
                    MaxDegreeOfParallelism = _extractorSettings.MaxDegreeOfParallelism, SingleProducerConstrained = true
                });
            return finalizerActionBlock;
        }

        private JoinBlock<string, string, string[]> GetJoinBlock()
        {
            var joinBlock = new JoinBlock<string, string, string[]>();
            return joinBlock;
        }

        private BroadcastBlock<string> GetBroadcastBlock()
        {
            var broadCast = new BroadcastBlock<string>(cloningFunc => cloningFunc, _executionDataflowBlockOptions);
            return broadCast;
        }

        #endregion
    }


    //TODO:config
    public class ExtractorSettings
    {
        public int MaxDegreeOfParallelism { get; set; }
        public string OutputFileFullPath { get; set; } //if not specified, there is no file will be written for output. Result will be kept in both case in memory.
        // ex: /User/temp/result.txt on macos  c:\temp\result.txt on windows//TODO: config
    }
}