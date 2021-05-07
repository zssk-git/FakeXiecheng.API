#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["FakeXiecheng.API/FakeXiecheng.API.csproj", "FakeXiecheng.API/"]
RUN dotnet restore "FakeXiecheng.API/FakeXiecheng.API.csproj"
COPY . .
WORKDIR "/src/FakeXiecheng.API"
RUN dotnet build "FakeXiecheng.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FakeXiecheng.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FakeXiecheng.API.dll"]