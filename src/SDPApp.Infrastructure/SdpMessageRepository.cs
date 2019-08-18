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
        private string _seperator;
        public SdpMessageRepository(string filePath,string seperator)
        {
            _filePath = filePath;
            _seperator = seperator;
        }

        
        public async Task<IEnumerable<string>> GetMessages()
        {
            var sdpFileContent = File.ReadAllText(_filePath); //TODO async read possible to make threadpool useful
            var seperator = _seperator; 
            var messages = sdpFileContent.Split(new[] {seperator}, StringSplitOptions.None);
            return await Task.FromResult(messages);
        }
    }
}