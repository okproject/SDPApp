using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SDPApp.Application.Abstraction;
using SDPApp.Core.Abstraction;

namespace SDPApp.Application.UseCase
{
    public class GetExtractedMessagesQueryHandler:IQueryHandler<GetExtractedMessagesQuery,GetExtractedMessageQueryViewModel>
    {
        private readonly ISdpMessageRepository _repository;
        private readonly ISdpExtractor _sdpExtractor;

        public GetExtractedMessagesQueryHandler(ISdpMessageRepository repository, ISdpExtractor sdpExtractor)
        {
            _repository = repository;
            _sdpExtractor = sdpExtractor;
        }

        public async Task<GetExtractedMessageQueryViewModel> Handle(GetExtractedMessagesQuery query, CancellationToken token)
        {
            var sdpMessages = await _repository.GetMessages();
            var extractedData = await _sdpExtractor.Extract(sdpMessages);
            
            var result=new GetExtractedMessageQueryViewModel()
            {
                ExtractedMessages = extractedData.ToList()
            };
            return result;
        }
    }
}