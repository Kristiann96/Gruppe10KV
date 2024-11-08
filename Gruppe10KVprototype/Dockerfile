FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Gruppe10KVprototype/Gruppe10KVprototype.csproj", "Gruppe10KVprototype/"]
COPY ["DataAccsess/DataAccess.csproj", "DataAccsess/"]
COPY ["Interfaces/Interfaces.csproj", "Interfaces/"]
COPY ["Models/Models.csproj", "Models/"]
COPY ["LogicInterfaces/LogicInterfaces.csproj", "LogicInterfaces/"]
COPY ["Logic/Logic.csproj", "Logic/"]
COPY ["ViewModels/ViewModels.csproj", "ViewModels/"]
COPY ["AuthInterface/AuthInterface.csproj", "AuthInterface/"]
COPY ["AuthDataAccess/AuthDataAccess.csproj", "AuthDataAccess/"]
RUN dotnet restore "Gruppe10KVprototype/Gruppe10KVprototype.csproj"
COPY . .
WORKDIR "/src/Gruppe10KVprototype"
RUN dotnet build "Gruppe10KVprototype.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Gruppe10KVprototype.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gruppe10KVprototype.dll"]
