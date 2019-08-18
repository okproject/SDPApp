using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SDPApp.Core.Abstraction;

namespace SDPApp.Infrastructure
{
    public class SdpMessageRepository:ISdpMessageRepository
    {
        private string _filePath;
        public SdpMessageRepository(string filePath)
        {
            _filePath = filePath;
        }

        
        public async Task<IEnumerable<string>> GetMessages()
        {
            var sdpFileContent = File.ReadAllText(_filePath); //TODO async read possible to make threadpool useful
            var seperator = $"\r\n\r\n";
            var messages = sdpFileContent.Split(new[] {seperator}, StringSplitOptions.RemoveEmptyEntries);
            return await Task.FromResult(messages);
        }
    }
}