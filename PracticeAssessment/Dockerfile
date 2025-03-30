# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the project files and restore any dependencies
COPY ./PracticeAssessment/ ./
RUN dotnet restore

# Build the app
RUN dotnet publish -c Release -o /app

# Use the official .NET runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .

# Expose the port the app runs on
EXPOSE 80

# Run the app
ENTRYPOINT ["dotnet", "PracticeAssessment.dll"]