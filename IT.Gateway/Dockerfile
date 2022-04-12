FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["IT.Gateway/IT.Gateway.csproj", "IT.Gateway/"]
RUN dotnet restore "IT.Gateway/IT.Gateway.csproj"
COPY . .
WORKDIR "/src/IT.Gateway"
RUN dotnet build "IT.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IT.Gateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IT.Gateway.dll"]
