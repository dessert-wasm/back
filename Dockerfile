# -----------------------------------------------------------
# Build
# -----------------------------------------------------------
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
WORKDIR /source

# Copy everything
COPY *.sln .
COPY src/ ./src/

RUN dotnet restore

WORKDIR /source/src/Dessert

# Build
RUN dotnet publish -c release -o /app --self-contained false --no-restore

# -----------------------------------------------------------
# Runtime
# -----------------------------------------------------------
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine
WORKDIR /app

# Binaries
COPY --from=build /app ./

ENTRYPOINT ["./Dessert", "start"]
