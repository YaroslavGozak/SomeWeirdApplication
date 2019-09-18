FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Handling.API/Handling.API.csproj", "Handling.API/"]
RUN dotnet restore "Handling.API/Handling.API.csproj"
COPY . .
WORKDIR "/src/Handling.API"
RUN dotnet build "Handling.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Handling.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Handling.API.dll"]