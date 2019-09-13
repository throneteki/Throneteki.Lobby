namespace Throneteki.LobbyNode.Services
{
    using CrimsonDev.Gameteki.LobbyNode.Config;
    using CrimsonDev.Gameteki.LobbyNode.Models;
    using CrimsonDev.Gameteki.LobbyNode.Services;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using StackExchange.Redis;

    public class ThronetekiLobbyService : LobbyService, IThronetekiLobbyService
    {
        public ThronetekiLobbyService(IConnectionMultiplexer redisConnection, IOptions<GametekiLobbyOptions> options, ILogger<LobbyService> logger)
            : base(redisConnection, options, logger)
        {
        }

        public GameResponse SelectDeck(string connectionId, PlayerDeck deck)
        {
            if (!UsersByConnectionId.ContainsKey(connectionId))
            {
                Logger.LogError($"Got select deck message for unknown connection id '{connectionId}'");
                return GameResponse.Failure("An error occurred selecting your deck.  Please try again later.");
            }

            var user = UsersByConnectionId[connectionId];
            var game = FindGameForUser(user.Name);
            if (game == null)
            {
                Logger.LogError($"Got select deck message for user '{user.Name}' with no game");
                return GameResponse.Failure("Could not select your deck because your game was not found.  Please restart your game.");
            }

            if (!game.GetPlayers().ContainsKey(user.Name))
            {
                Logger.LogError($"Got select deck message for user '{user.Name}' game '{game.Id}' which they are not in.");
                return GameResponse.Failure("An error occurred selecting your deck.  Please try again later.");
            }

            var player = game.GetPlayers()[user.Name];

            var customData = JsonConvert.DeserializeObject<PlayerCustomData>(player.CustomData) ?? new PlayerCustomData();
            customData.Deck = deck;

            player.CustomData = JsonConvert.SerializeObject(customData);

            return GameResponse.Succeeded(game);
        }
    }
}
