FROM mcr.microsoft.com/dotnet/core/sdk:2.1.811 AS build

# Copy csproj and restore
WORKDIR /app
COPY Directory.Build.props .
COPY Azure.Test.PerfStress/*.csproj Azure.Test.PerfStress/
COPY Azure.Storage.Blobs.V11.PerfStress/*.csproj Azure.Storage.Blobs.V11.PerfStress/

WORKDIR /app/Azure.Storage.Blobs.V11.PerfStress
RUN dotnet restore

# Copy code and publish
WORKDIR /app
COPY Azure.Test.PerfStress Azure.Test.PerfStress/
COPY Azure.Storage.Blobs.V11.PerfStress Azure.Storage.Blobs.V11.PerfStress/

WORKDIR /app/Azure.Storage.Blobs.V11.PerfStress
RUN dotnet publish --no-restore -c Release -o out -f netcoreapp2.1


# Copy build artifacts into runtime image
FROM mcr.microsoft.com/dotnet/core/runtime:2.1.23 AS runtime
WORKDIR /app
COPY --from=build /app/Azure.Storage.Blobs.V11.PerfStress/out ./
ENTRYPOINT ["dotnet", "./Azure.Storage.Blobs.V11.PerfStress.dll"]
