# Base runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files first (for caching)
COPY src/BookMS.WebApi/BookMS.WebApi.csproj src/BookMS.WebApi/
COPY src/BookMS.Infrastructure/BookMS.Infrastructure.csproj src/BookMS.Infrastructure/
COPY src/BookMS.Domain/BookMS.Domain.csproj src/BookMS.Domain/
COPY src/BookMS.Application/BookMS.Application.csproj src/BookMS.Application/
RUN dotnet restore src/BookMS.WebApi/BookMS.WebApi.csproj

# Copy the rest and publish
COPY . .
WORKDIR /src/src/BookMS.WebApi
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BookMS.WebApi.dll"]
