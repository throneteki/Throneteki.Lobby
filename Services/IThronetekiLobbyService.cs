namespace Throneteki.LobbyNode.Services
{
    using CrimsonDev.Gameteki.LobbyNode.Models;
    using CrimsonDev.Gameteki.LobbyNode.Services;

    public interface IThronetekiLobbyService : ILobbyService
    {
        GameResponse SelectDeck(string connectionId, PlayerDeck deck);
    }
}
