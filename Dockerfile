FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

# ENV TZ=Asia/Shanghai
ENV ASPNETCORE_ENVIRONMENT=$ASPNETCORE_ENVIRONMENT

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["FundApp/FundApp.csproj", "FundApp/"]
COPY ["Service/Service.csproj", "Service/"]
RUN dotnet restore "FundApp/FundApp.csproj"
COPY . .

WORKDIR "/src/FundApp"
RUN dotnet build "FundApp.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "FundApp.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "FundApp.dll"]
