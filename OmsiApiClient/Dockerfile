FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["OmsiApiClient/OmsiApiClient.csproj", "OmsiApiClient/"]
RUN dotnet restore "OmsiApiClient/OmsiApiClient.csproj"
COPY . .
WORKDIR "/src/OmsiApiClient"
RUN dotnet build "OmsiApiClient.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OmsiApiClient.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OmsiApiClient.dll"]
