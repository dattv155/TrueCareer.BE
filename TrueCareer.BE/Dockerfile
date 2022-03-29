FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["TrueCareer.BE.csproj", "./"]
RUN dotnet restore "TrueCareer.BE.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "TrueCareer.BE.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TrueCareer.BE.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 88

COPY ["docker-entrypoint.sh", "."]

RUN chmod a+x docker-entrypoint.sh

CMD ["./docker-entrypoint.sh"]

