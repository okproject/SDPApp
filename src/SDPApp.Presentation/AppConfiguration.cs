using System;

namespace SDPApp.Presentation
{
    public class AppConfiguration
    {
        public string OutPutFileFullPath { get; set; }
        public int ProcessorCount { get; set; }
        public string SourceFilePathForSdpMessages { get; set; } //The file that contains source sdp messages

        public string
            SourceFileSeperator { get; set; } // Seperator for Source sdp file to seperate message for double new line

        public AppConfiguration()
        {
            OutPutFileFullPath = GetNewOutputFilePath();
            ProcessorCount = Environment.ProcessorCount;
            SourceFilePathForSdpMessages =
                @"/Users/rasihcaglayan/Documents/temp/base/sdp_input_huge.txt"; //source sdp message file. this file will be using extracting data.
            SourceFileSeperator = $"\r\n\r\n";
            ;
        }

        public string GetNewOutputFilePath()
        {
            return $"{Environment.CurrentDirectory}/result-{Guid.NewGuid().ToString()}.txt"; //File will be saved here;
        }
    }
}