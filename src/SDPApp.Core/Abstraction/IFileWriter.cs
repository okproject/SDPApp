using System.Collections.Generic;

namespace SDPApp.Core.Abstraction
{
    public interface IFileWriter
    {
        void WriteToFile(string fullFilePathToSaveData, IEnumerable<string> data);
    }
}