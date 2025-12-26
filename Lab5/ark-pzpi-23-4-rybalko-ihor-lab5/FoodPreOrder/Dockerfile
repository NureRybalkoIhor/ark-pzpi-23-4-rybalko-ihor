FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore "FoodPreOrder.sln"

WORKDIR "/src/FoodPreOrder.Api"
RUN dotnet build "FoodPreOrder.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FoodPreOrder.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FoodPreOrder.Api.dll"]