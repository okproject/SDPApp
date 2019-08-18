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
            try
            {
                if (string.IsNullOrEmpty(fullFilePathToSaveData) &&
                    Directory.Exists(Path.GetFullPath(fullFilePathToSaveData)))
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