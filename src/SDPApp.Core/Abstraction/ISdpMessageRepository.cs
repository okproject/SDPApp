using System.Collections.Generic;

namespace SDPApp.Core.Abstraction
{
    public interface ISdpMessageRepository
    {
        IEnumerable<string> GetMessages();
    }
}