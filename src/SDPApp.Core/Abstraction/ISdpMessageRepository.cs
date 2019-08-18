using System.Collections.Generic;
using System.Threading.Tasks;

namespace SDPApp.Core.Abstraction
{
    public interface ISdpMessageRepository
    {
        Task<IEnumerable<string>> GetMessages();
    }
}