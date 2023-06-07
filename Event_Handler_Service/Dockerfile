# Use the official .NET 6 SDK as the base image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Set the working directory inside the container
WORKDIR /app

# Copy the project files to the working directory
COPY ./Common/Common.csproj ./Common/
COPY . ./

# Build the solution including the "Common" class library
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Use a lightweight runtime image as the base image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime

# Set the working directory inside the container
WORKDIR /app

# Copy the built application from the build stage
COPY --from=build /app/out ./

# Expose port 7278 for Swagger UI
EXPOSE 7152

# Set the entry point for the container
ENTRYPOINT ["dotnet", "Event_Handler_Service.dll"]

# wroking docker command with this: docker build -t s6event-handler-service -f Event_Handler_Service/Dockerfile .