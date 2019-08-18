namespace SDPApp.Core.Abstraction
{
    public interface IPortParser
    {
        string ParsePort(string rawSdpMessage);
    }
}