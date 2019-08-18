using System;
using System.Threading;
using System.Threading.Tasks;
using SDPApp.Application.UseCase;
using SDPApp.Infrastructure;
using SDPApp.Infrastructure.Parsers;

namespace SDPApp.Presentation
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //Configure app here
            var outputPath =
                $"{Environment.CurrentDirectory}/result-{Guid.NewGuid().ToString()}.txt"; //File will be saved here
            var processorCount = Environment.ProcessorCount; //processor count here
            var inputPath =
                @"/Users/rasihcaglayan/Documents/temp/base/sdp_input_huge.txt"; //source sdp message file. this file will be using extracting data.

            var inputFileMessageSeperator=$"\r\n\r\n";; //IMPORTANT: This line may need to change on different operating systems; new line character. We parse source file by double new line
            
            //After configuration, it is ready to run here
            await ExtractData(processorCount, inputPath, outputPath,inputFileMessageSeperator);
            Console.WriteLine("Finished! Presss a key to exit.");
            Console.ReadKey();
        }

        public static async Task ExtractData(int maxDegreeOfParallelism, string inputFileMessageSeperator,
            string extractedMessagesOutputPath,string seperator)
        {
            var extractorSettings = new ExtractorSettings
            {
                MaxDegreeOfParallelism = maxDegreeOfParallelism,
                OutputFileFullPath = extractedMessagesOutputPath
            };
            var sdpMessagesFilePath = inputFileMessageSeperator;


            var ipParser = new IpParser();
            var portParser = new PortParser();
            var codecParser = new CodecParser();
            var repository = new SdpMessageRepository(sdpMessagesFilePath,seperator);
            var fileWriter = new ResultFileWriter();
            var _sdpExtractor = new SdpExtractor(ipParser, portParser, codecParser, extractorSettings, fileWriter);
            var query = new GetExtractedMessagesQuery();
            var handler = new GetExtractedMessagesQueryHandler(repository, _sdpExtractor);
            var extractedData = await handler.Handle(query, new CancellationToken());

            //YOU CAN OBSERVER RESULT IN THIS VARIABLE WITH BREAKPOINT: resultMessages
            var resultMessages =
                extractedData
                    .ExtractedMessages; //results will be saved path specified in extractorSettings.OutputFileFullPath variable; you can find it from starting lines of this method.

            Console.WriteLine($"File saved to: {extractorSettings.OutputFileFullPath}"); //Find where file saved
            Console.WriteLine(
                $"Output format : IP;PORT;codec1-codec2-codec-n"); // This is the format that i am using to save file. It has same order with source data.
        }
    }
}