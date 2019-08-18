namespace SDPApp.Core.Abstraction
{
    public interface IIPParser
    {
        string ParseIp(string rawSdpMessage);
    }
}