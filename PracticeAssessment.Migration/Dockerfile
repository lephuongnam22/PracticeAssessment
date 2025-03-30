# Set the base image to the .NET SDK for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

# Set working directory
WORKDIR /app

# Copy the project file
COPY ./PracticeAssessment.Migration/ ./

# Restore dependencies
RUN dotnet restore

# Set the runtime image
FROM mcr.microsoft.com/dotnet/runtime:8.0-alpine

# Install rsync for copying files
RUN apk add --no-cache rsync

# Set working directory
WORKDIR /app

COPY --from=build /app .

# Run the migration
ENTRYPOINT ["dotnet", "PracticeAssessment.Migration"]