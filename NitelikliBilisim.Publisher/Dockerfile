#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["NitelikliBilisim.Publisher/NitelikliBilisim.Publisher.csproj", "NitelikliBilisim.Publisher/"]
COPY ["NitelikliBilisim.Business/NitelikliBilisim.Business.csproj", "NitelikliBilisim.Business/"]
COPY ["NitelikliBilisim.Data/NitelikliBilisim.Data.csproj", "NitelikliBilisim.Data/"]
COPY ["NitelikliBilisim.Core/NitelikliBilisim.Core.csproj", "NitelikliBilisim.Core/"]
COPY ["NitelikliBilisim.Support/NitelikliBilisim.Support.csproj", "NitelikliBilisim.Support/"]
RUN dotnet restore "NitelikliBilisim.Publisher/NitelikliBilisim.Publisher.csproj"
COPY . .
WORKDIR "/src/NitelikliBilisim.Publisher"
RUN dotnet build "NitelikliBilisim.Publisher.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NitelikliBilisim.Publisher.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NitelikliBilisim.Publisher.dll"]