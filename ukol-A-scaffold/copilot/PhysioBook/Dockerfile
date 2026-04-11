FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY PhysioBook.csproj ./
RUN dotnet restore PhysioBook.csproj

COPY . .
RUN dotnet publish PhysioBook.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:5050
EXPOSE 5050
ENTRYPOINT ["dotnet", "PhysioBook.dll"]

