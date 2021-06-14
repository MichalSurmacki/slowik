FROM mcr.microsoft.com/dotnet/core/sdk:3.1-bionic AS build-env
WORKDIR /app

COPY . ./

RUN dotnet restore
RUN dotnet publish Api -c Release -o out

RUN dotnet tool install --global dotnet-ef

# FROM mcr.microsoft.com/dotnet/aspnet:3.1
# WORKDIR /app
# COPY --from=build-env /app/out .

RUN chmod +x docker-entrypoint.sh
ENTRYPOINT ./docker-entrypoint.sh