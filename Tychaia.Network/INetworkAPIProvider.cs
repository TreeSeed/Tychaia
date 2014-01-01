namespace Tychaia.Network
{
    public interface INetworkAPIProvider
    {
        bool IsAvailable { get; }

        INetworkAPI GetAPI();
    }
}
