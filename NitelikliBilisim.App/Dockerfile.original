FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["NitelikliBilisim.App/NitelikliBilisim.App.csproj", "NitelikliBilisim.App/"]
RUN dotnet restore "NitelikliBilisim.App/NitelikliBilisim.App.csproj"
COPY . .
WORKDIR "/src/NitelikliBilisim.App"
RUN dotnet build "NitelikliBilisim.App.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "NitelikliBilisim.App.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "NitelikliBilisim.App.dll"]