FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["OmsiClient/OmsiClient.csproj", "OmsiClient/"]
RUN dotnet restore "OmsiClient/OmsiClient.csproj"
COPY . .
WORKDIR "/src/OmsiClient"
RUN dotnet build "OmsiClient.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OmsiClient.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OmsiClient.dll"]
