#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["BidMasterOnline.API/BidMasterOnline.API.csproj", "BidMasterOnline.API/"]
COPY ["BidMasterOnline.Application/BidMasterOnline.Application.csproj", "BidMasterOnline.Application/"]
COPY ["BidMasterOnline.Domain/BidMasterOnline.Domain.csproj", "BidMasterOnline.Domain/"]
COPY ["BidMasterOnline.Infrastructure/BidMasterOnline.Infrastructure.csproj", "BidMasterOnline.Infrastructure/"]
RUN dotnet restore "BidMasterOnline.API/BidMasterOnline.API.csproj"
COPY . .
WORKDIR "/src/BidMasterOnline.API"
RUN dotnet build "BidMasterOnline.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BidMasterOnline.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BidMasterOnline.API.dll"]