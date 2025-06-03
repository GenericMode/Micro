# Use the official ASP.NET Core runtime as a base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
RUN apt-get update && apt-get install -y iputils-ping
EXPOSE 80
EXPOSE 443
EXPOSE 6020
ENV ASPNETCORE_URLS=http://+:6020

# Use the SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["OrderAPI.csproj", "./"]
RUN dotnet restore "OrderAPI.csproj"
# Copy everything
COPY . .
# -c Debug instead of -c Release - temp solution because of Swagger
RUN dotnet publish "OrderAPI.csproj" -c Debug -o /app/publish 

# Build runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OrderAPI.dll"]