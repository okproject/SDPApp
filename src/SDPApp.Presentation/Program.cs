using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SDPApp.Application.UseCase;
using SDPApp.Core.Abstraction;
using SDPApp.Infrastructure;
using SDPApp.Infrastructure.Parsers;

namespace SDPApp.Presentation
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = new AppConfiguration(); //you can change config inside file

            //====== Try these 3 methods. All doing same thing with different way====

            //==Strategy 1: TPL Dataflow
            await ExtractData(config.ProcessorCount, config.SourceFilePathForSdpMessages, config.OutPutFileFullPath,
                config.SourceFileSeperator, RunTimeMode.TplDataFlow);

            //==Strategy 2: Parallel Linq===
            config.OutPutFileFullPath = config.GetNewOutputFilePath();
            await ExtractData(config.ProcessorCount, config.SourceFilePathForSdpMessages, config.OutPutFileFullPath,
                config.SourceFileSeperator, RunTimeMode.Parallel);

            //===Strategy 3: Sequential
            config.OutPutFileFullPath = config.GetNewOutputFilePath();
            await ExtractData(config.ProcessorCount, config.SourceFilePathForSdpMessages, config.OutPutFileFullPath,
                config.SourceFileSeperator, RunTimeMode.Sequential);

            Console.WriteLine("Finished! Presss a key to exit.");
            Console.ReadKey();
        }


        private static async Task ExtractData(int maxDegreeOfParallelism, string sdpMessagesFilePath,
            string extractedMessagesOutputPath, string seperator, RunTimeMode mode)
        {
            #region Configuration And Composition

            var extractorSettings = new ExtractorSettings
            {
                MaxDegreeOfParallelism = maxDegreeOfParallelism,
                OutputFileFullPath = extractedMessagesOutputPath
            };
            


            var ipParser = new IpParser();
            var portParser = new PortParser();
            var codecParser = new CodecParser();
            var repository = new SdpMessageRepository(sdpMessagesFilePath, seperator);
            var fileWriter = new ResultFileWriter();

            ISdpExtractor extractor;
            //
            switch (mode)
            {
                case RunTimeMode.Parallel:
                    extractor =
                        new SdpExtractorParallelLinq(fileWriter, ipParser, portParser, codecParser, extractorSettings);
                    break;
                case RunTimeMode.Sequential:
                    extractor = new SdpExtractorSequential(fileWriter, ipParser, portParser, codecParser,
                        extractorSettings);
                    break;
                case RunTimeMode.TplDataFlow:
                    extractor = new SdpExtractorTplDataFlow(ipParser, portParser, codecParser, extractorSettings,
                        fileWriter);
                    break;
                default:
                    throw new Exception("Mode not found");
            }

            #endregion

            
            var query = new GetExtractedMessagesQuery();
            var handler = new GetExtractedMessagesQueryHandler(repository, extractor);
            var extractedData = await handler.Handle(query, new CancellationToken());

            var resultMessages =
                extractedData
                    .ExtractedMessages; //results will be saved path specified in extractorSettings.OutputFileFullPath variable; you can find it from starting lines of this method.

            Console.WriteLine($"File saved to: {extractorSettings.OutputFileFullPath}"); //Find where file saved
            Console.WriteLine(
                $"Output format : IP;PORT;codec1-codec2-codec-n"); // This is the format that i am using to save file. It has same order with source data.
        }
    }
}