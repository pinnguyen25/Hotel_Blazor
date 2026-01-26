# 1. Base Image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG configuration=Release
WORKDIR /src

COPY ["HotelBooking.api/HotelBooking.api.csproj", "HotelBooking.api/"]
COPY ["HotelBooking.application/HotelBooking.application.csproj", "HotelBooking.application/"]
COPY ["HotelBooking.infrastructure/HotelBooking.infrastructure.csproj", "HotelBooking.infrastructure/"]
COPY ["HotelBooking.webapp/HotelBooking.webapp.csproj", "HotelBooking.webapp/"]

RUN dotnet restore "HotelBooking.webapp/HotelBooking.webapp.csproj"

COPY . .
WORKDIR "/src/HotelBooking.webapp"
RUN dotnet build "HotelBooking.webapp.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "HotelBooking.webapp.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HotelBooking.webapp.dll"]