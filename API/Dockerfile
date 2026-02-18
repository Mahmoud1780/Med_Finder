FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY MedicineFinder.sln ./
COPY API/MedicineFinder.API.csproj API/
COPY Application/MedicineFinder.Application.csproj Application/
COPY Domain/MedicineFinder.Domain.csproj Domain/
COPY Infrastructure/MedicineFinder.Infrastructure.csproj Infrastructure/
RUN dotnet restore API/MedicineFinder.API.csproj

COPY . .
RUN dotnet publish API/MedicineFinder.API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:5018
EXPOSE 5018
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MedicineFinder.API.dll"]
