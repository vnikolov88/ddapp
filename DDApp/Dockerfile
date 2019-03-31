FROM microsoft/dotnet:2.2-aspnetcore-runtime-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk-stretch AS build
WORKDIR /src
COPY ["DDApp/DDApp.csproj", "DDApp/"]
RUN dotnet restore "DDApp/DDApp.csproj"
COPY . .
WORKDIR "/src/DDApp"
RUN dotnet build "DDApp.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "DDApp.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DDApp.dll"]