FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["CnabProcessor/CnabProcessor.csproj", "CnabProcessor/"]
RUN dotnet restore "CnabProcessor/CnabProcessor.csproj"
COPY . .
WORKDIR "/src/CnabProcessor"
RUN dotnet build "CnabProcessor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CnabProcessor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CnabProcessor.dll"]


