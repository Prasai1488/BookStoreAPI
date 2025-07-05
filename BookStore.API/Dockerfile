# Stage 1: Build the application
FROM mcr.microsoft.com / dotnet / sdk:7.0 AS build
WORKDIR /app

# Copy csproj and restore dependencies
COPY *.sln .
COPY BookStore.API/*.csproj ./BookStore.API/
RUN dotnet restore

# Copy the remaining source code and build the app
COPY BookStore.API/. ./BookStore.API/
WORKDIR /app/BookStore.API
RUN dotnet publish -c Release -o out

# Stage 2: Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/BookStore.API/out ./

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "BookStore.API.dll"]
