FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY APIWithJwt/APIWithJwt.csproj APIWithJwt/
RUN dotnet restore APIWithJwt/APIWithJwt.csproj
COPY . .
WORKDIR /src/APIWithJwt
RUN dotnet build APIWithJwt.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish APIWithJwt.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "APIWithJwt.dll"]
