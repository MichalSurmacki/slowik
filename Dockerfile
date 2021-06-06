FROM mcr.microsoft.com/dotnet/core/sdk:3.1-bionic
WORKDIR /app

COPY . ./

RUN dotnet restore
RUN dotnet publish Api -c Release -o out

RUN dotnet tool install --global dotnet-ef

RUN chmod +x docker-entrypoint.sh
ENTRYPOINT ./docker-entrypoint.sh