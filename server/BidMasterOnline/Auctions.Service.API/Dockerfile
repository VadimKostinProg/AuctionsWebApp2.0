#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Auctions.Service.API/Auctions.Service.API.csproj", "Auctions.Service.API/"]
COPY ["BidMasterOnline.Core/BidMasterOnline.Core.csproj", "BidMasterOnline.Core/"]
COPY ["BidMasterOnline.Domain/BidMasterOnline.Domain.csproj", "BidMasterOnline.Domain/"]
COPY ["BidMasterOnline.Infrastructure/BidMasterOnline.Infrastructure.csproj", "BidMasterOnline.Infrastructure/"]
RUN dotnet restore "./Auctions.Service.API/Auctions.Service.API.csproj"
COPY . .
WORKDIR "/src/Auctions.Service.API"
RUN dotnet build "./Auctions.Service.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Auctions.Service.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Auctions.Service.API.dll"]