# Use the official .NET SDK image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy project files
COPY . .

# Restore dependencies and build
RUN dotnet restore
RUN dotnet publish -c Release -o /out

# Use a smaller runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /out ./

# Expose port (not needed for Render, but good practice)
EXPOSE 5000

# Run the app
CMD ["dotnet", "TodoApi.dll"]
