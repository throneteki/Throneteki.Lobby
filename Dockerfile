FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY *.csproj Throneteki.Lobby/
COPY Throneteki.Data Throneteki.Lobby/Throneteki.Data
RUN dotnet restore Throneteki.Lobby/Throneteki.LobbyNode.csproj
COPY . /src/Throneteki.Lobby

WORKDIR /src/Throneteki.Lobby
RUN dotnet build -c Release -o /app

FROM build as publish
RUN dotnet publish -c Release -o /app -f netcoreapp2.2

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

ENTRYPOINT ["dotnet", "Throneteki.LobbyNode.dll"]