FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR "/APIWithJwt"
COPY ["APIWithJwt.csproj", "APIWithJwt/"]

RUN dotnet restore "APIWithJwt/APIWithJwt.csproj"
COPY . .
WORKDIR "/APIWithJwt"
RUN dotnet build "APIWithJwt.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "APIWithJwt.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "APIWithJwt.dll"]