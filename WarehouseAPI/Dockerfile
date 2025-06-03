# Use the official ASP.NET Core runtime as a base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
RUN apt-get update && apt-get install -y iputils-ping
RUN chmod -R 775 /app
EXPOSE 80
EXPOSE 443
EXPOSE 5221
ENV ASPNETCORE_URLS=http://+:5221

# Use the SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the csproj files explicitly
#COPY ../OrderAPI/OrderAPI.csproj ./OrderAPI/OrderAPI.csproj
#COPY WarehouseAPI/WarehouseAPI.csproj ./WarehouseAPI/WarehouseAPI.csproj

COPY ["OrderAPI/OrderAPI.csproj", "OrderAPI/"]
COPY ["WarehouseAPI/WarehouseAPI.csproj", "WarehouseAPI/"]

#COPY ../Micro.sln ./

# Restore dependencies
#WORKDIR /src/WarehouseAPI
RUN dotnet restore "WarehouseAPI/WarehouseAPI.csproj"
RUN dotnet restore "OrderAPI/OrderAPI.csproj"


#copy the everything else
#COPY ../OrderAPI ./OrderAPI
#COPY "WarehouseAPI" "/WarehouseAPI"
COPY "WarehouseAPI/" "WarehouseAPI/"
COPY "OrderAPI/" "OrderAPI/"

# -c Debug instead of -c Release - temp solution because of Swagger
# Publish the app to a writable location
WORKDIR "/src/WarehouseAPI"
RUN dotnet publish "WarehouseAPI.csproj" -c Debug -o /app/publish 
RUN ls -la /app/publish

# Build runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish/ ./
ENTRYPOINT ["dotnet", "WarehouseAPI.dll"]