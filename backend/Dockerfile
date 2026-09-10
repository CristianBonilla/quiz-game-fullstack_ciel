# syntax=docker/dockerfile:1

# --- build ---
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files and shared MSBuild props first so `dotnet restore` is cached
# independently of source-code changes (layer cache is invalidated only when a
# .csproj/.props file actually changes).
COPY Directory.Build.props Directory.Packages.props ./
COPY src/QuizGame.Domain/QuizGame.Domain.csproj src/QuizGame.Domain/
COPY src/QuizGame.Application/QuizGame.Application.csproj src/QuizGame.Application/
COPY src/QuizGame.Infrastructure/QuizGame.Infrastructure.csproj src/QuizGame.Infrastructure/
COPY src/QuizGame.Api/QuizGame.Api.csproj src/QuizGame.Api/
RUN dotnet restore src/QuizGame.Api/QuizGame.Api.csproj

COPY src/ src/
RUN dotnet publish src/QuizGame.Api/QuizGame.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish

# --- runtime ---
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# curl is required by the HEALTHCHECK below; the base runtime image doesn't ship it.
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# The "app" user already exists in Microsoft's ASP.NET Core images (non-root by design).
USER app

HEALTHCHECK --interval=15s --timeout=5s --start-period=20s --retries=5 \
    CMD curl --fail http://localhost:8080/health/live || exit 1

ENTRYPOINT ["dotnet", "QuizGame.Api.dll"]
