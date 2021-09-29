#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS publish
WORKDIR /src
COPY ["Utils.csproj", "./"]
RUN dotnet restore "Utils.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "Utils.csproj" -c Release -o /app/build
RUN dotnet publish "Utils.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:3.1

EXPOSE 8080
WORKDIR /app
# RUN apt-get update && apt-get install -y net-tools curl iputils-ping telnetd telnet nano vim libc6-dev libgdiplus dnsutils

COPY --from=publish /app/publish .
RUN chmod -R g+w /app

COPY ["docker-entrypoint.sh", "."]
RUN chmod a+x docker-entrypoint.sh
CMD ["./docker-entrypoint.sh"]

