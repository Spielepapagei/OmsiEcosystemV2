FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["OmsiApiServer/OmsiApiServer.csproj", "OmsiApiServer/"]
RUN dotnet restore "OmsiApiServer/OmsiApiServer.csproj"
COPY . .
WORKDIR "/src/OmsiApiServer"
RUN dotnet build "OmsiApiServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OmsiApiServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OmsiApiServer.dll"]
