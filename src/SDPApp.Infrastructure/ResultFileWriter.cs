using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SDPApp.Core.Abstraction;

namespace SDPApp.Infrastructure
{
    public class ResultFileWriter : IFileWriter
    {
        public void WriteToFile(string fullFilePathToSaveData, IEnumerable<string> data)
        {
            if (string.IsNullOrEmpty(fullFilePathToSaveData)) return;
            try
            {
                var path = Path.GetDirectoryName(fullFilePathToSaveData);

                if (Directory.Exists(path))
                {
                    File.WriteAllLines(fullFilePathToSaveData, data, Encoding.UTF8);
                }
                else
                {
                    Console.WriteLine("Directory could not found; skipping write result to file step");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"File could not saved!: {e}");
            }
        }
    }
}