FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
FROM node:6.10.3
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["FundApp/FundApp.csproj", "FundApp/"]
COPY ["Service/Service.csproj", "Service/"]
RUN dotnet restore "FundApp/FundApp.csproj"
COPY . .

RUN /app/configure
RUN npm run build

WORKDIR "/src/FundApp"
RUN dotnet build "FundApp.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "FundApp.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "FundApp.dll"]