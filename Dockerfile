FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copia as soluções e projetos
COPY *.sln .
COPY src/SneakerClean.API/*.csproj src/SneakerClean.API/
COPY src/SneakerClean.Application/*.csproj src/SneakerClean.Application/
COPY src/SneakerClean.Domain/*.csproj src/SneakerClean.Domain/
COPY src/SneakerClean.Infrastructure/*.csproj src/SneakerClean.Infrastructure/

RUN dotnet restore src/SneakerClean.API/SneakerClean.API.csproj

COPY . .
WORKDIR /app/src/SneakerClean.API
RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /out .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "SneakerClean.API.dll"]