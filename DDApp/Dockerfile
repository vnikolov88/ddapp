FROM microsoft/dotnet:2.2-aspnetcore-runtime-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM node:6.4.0 AS npm-tasks
WORKDIR /src
COPY . .
WORKDIR "/src/DDApp"
RUN npm install
RUN npm rebuild node-sass
RUN npm install gulp@3.8.11 -g
RUN gulp default
WORKDIR /src

FROM microsoft/dotnet:2.2-sdk-stretch AS build
COPY --from=npm-tasks . .
WORKDIR /src
COPY ["DDApp/DDApp.csproj", "DDApp/"]
RUN dotnet restore "DDApp/DDApp.csproj"

WORKDIR "/src/DDApp"
RUN dotnet build "DDApp.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "DDApp.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DDApp.dll"]