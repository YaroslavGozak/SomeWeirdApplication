FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Products.API/Products.API.csproj", "Products.API/"]
RUN dotnet restore "Products.API/Products.API.csproj"
COPY . .
WORKDIR "/src/Products.API"
RUN dotnet build "Products.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Products.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Products.API.dll"]