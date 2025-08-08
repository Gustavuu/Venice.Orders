FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Venice.Orders.sln", "."]

COPY ["Venice.Orders.API/Venice.Orders.API.csproj", "Venice.Orders.API/"]
COPY ["Venice.Orders.Application/Venice.Orders.Application.csproj", "Venice.Orders.Application/"]
COPY ["Venice.Orders.Domain/Venice.Orders.Domain.csproj", "Venice.Orders.Domain/"]
COPY ["Venice.Orders.Infrastructure/Venice.Orders.Infrastructure.csproj", "Venice.Orders.Infrastructure/"]
COPY ["Venice.Orders.Tests/Venice.Orders.Tests.csproj", "Venice.Orders.Tests/"]

RUN dotnet restore "Venice.Orders.sln"

COPY . .

WORKDIR "/src/Venice.Orders.API"
RUN dotnet publish "Venice.Orders.API.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Venice.Orders.API.dll"]