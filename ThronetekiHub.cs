namespace Throneteki.LobbyNode
{
    using System;
    using System.Threading.Tasks;
    using CrimsonDev.Gameteki.Data.Models.Api;
    using CrimsonDev.Gameteki.LobbyNode.Config;
    using CrimsonDev.Gameteki.LobbyNode.Hubs;
    using CrimsonDev.Gameteki.LobbyNode.Services;
    using CrimsonDev.Throneteki.Data.Models.Api;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using StackExchange.Redis;
    using Throneteki.LobbyNode.Services;

    public class ThronetekiHub : LobbyHub
    {
        private readonly IThronetekiLobbyService thronetekiLobbyService;
        private readonly IHttpClient httpClient;
        private readonly GametekiLobbyOptions options;

        public ThronetekiHub(
            IThronetekiLobbyService lobbyService,
            IConnectionMultiplexer redisConnection,
            IGameNodeService gameNodeService,
            IOptions<AuthTokenOptions> tokenOptions,
            IHttpClient httpClient,
            IOptions<GametekiLobbyOptions> options)
            : base(lobbyService, redisConnection, gameNodeService, tokenOptions)
        {
            this.httpClient = httpClient;
            this.options = options.Value;
            thronetekiLobbyService = lobbyService;
        }

        [HubMethodName("selectdeck")]
        [Authorize]
        public async Task SelectDeckAsync(Guid gameId, int deckId)
        {
            var userData = Context.User.FindFirst("UserData");
            if (userData == null)
            {
                await Clients.Caller.JoinFailed("An error occurred selecting your deck.  Please clear your browser cache and try again.");

                return;
            }

            var apiUser = JsonConvert.DeserializeObject<ApiUser>(userData.Value);
            if (apiUser == null)
            {
                await Clients.Caller.JoinFailed("An error occurred selecting your deck.  Please clear your browser cache and try again.");

                return;
            }

            var result = await httpClient.GetJsonObjectAsync<ValidateDeckForUserResponse>($"{options.ApiUrl}api/decks/{deckId}/validate/{apiUser.Id}");
            if (result == null || !result.Success)
            {
                await Clients.Caller.JoinFailed("An error occurred validating your deck.  Please try again later.");

                return;
            }

            var gameResponse = thronetekiLobbyService.SelectDeck(Context.ConnectionId, new PlayerDeck { Id = deckId, ValidationResult = result.Result });
            if (!gameResponse.Success)
            {
                await Clients.Caller.JoinFailed(gameResponse.Message);

                return;
            }

            await SendGameStateAsync(gameResponse.Game);
        }
    }
}
