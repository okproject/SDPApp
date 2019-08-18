using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SDPApp.Core
{
    public class ExtractedMessage
    {
        public string Ip { get; set; }
        public string Port { get; set; }
        public IEnumerable<string> Codecs { get; set; }

        public override string ToString()
        {
            var result = $"{Ip};{Port};{string.Join("-", Codecs)}";
            result = Regex.Replace(result, @"\t|\n|\r", "");
            return result;
        }
    }
}