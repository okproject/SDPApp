using System.Collections.Generic;
using SDPApp.Core;

namespace SDPApp.Application.UseCase
{
    public class GetExtractedMessageQueryViewModel
    {
        public IEnumerable<ExtractedMessage> ExtractedMessages { get; set; }
    }
}