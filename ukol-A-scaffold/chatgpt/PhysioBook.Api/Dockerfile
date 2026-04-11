FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["PhysioBook.Api/PhysioBook.Api.csproj", "PhysioBook.Api/"]
RUN dotnet restore "PhysioBook.Api/PhysioBook.Api.csproj"

COPY . .
WORKDIR /src/PhysioBook.Api
RUN dotnet publish "PhysioBook.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "PhysioBook.Api.dll"]
