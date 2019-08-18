using System;
using System.Threading;
using System.Threading.Tasks;
using SDPApp.Application.UseCase;
using SDPApp.Core.Abstraction;
using SDPApp.Infrastructure;

namespace SDPApp.Presentation
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        public static async Task ExtractData()
        {
            var sdpMessagesFilePath = "";
            ISdpMessageRepository repository = new SdpMessageRepository(sdpMessagesFilePath);
            ISdpExtractor _sdpExtractor;

            var query = new GetExtractedMessagesQuery();
//            var handler = new GetExtractedMessagesQueryHandler(repository, _sdpExtractor);
//            var extractedData = await handler.Handle(query, new CancellationToken());
        }
    }
}