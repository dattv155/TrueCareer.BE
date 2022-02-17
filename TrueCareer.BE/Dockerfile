FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS base
WORKDIR /app
RUN apt-get update && apt-get install -y net-tools curl iputils-ping telnetd nano vim libc6-dev libgdiplus

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
ENV TZ=Asia/Ho_Chi_Minh
RUN ln -snf /usr/share/zoneinfo/Asia/Ho_Chi_Minh /etc/localtime && echo Asia/Ho_Chi_Minh > /etc/timezone
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 88

COPY ["docker-entrypoint.sh", "."]

RUN chmod a+x docker-entrypoint.sh

CMD ["./docker-entrypoint.sh"]

