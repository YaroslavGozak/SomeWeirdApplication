FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["SomeWeirdApplicationBackend/SomeWeirdApplicationBackend.csproj", "SomeWeirdApplicationBackend/"]
RUN dotnet restore "SomeWeirdApplicationBackend/SomeWeirdApplicationBackend.csproj"
COPY . .
WORKDIR "/src/SomeWeirdApplicationBackend"
RUN dotnet build "SomeWeirdApplicationBackend.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "SomeWeirdApplicationBackend.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SomeWeirdApplicationBackend.dll"]
