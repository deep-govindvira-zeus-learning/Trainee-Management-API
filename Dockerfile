# Build stage using the corporate devcontainer image
FROM docker-registry-002.zeuslearning.com/zeuslearning/vscode/devcontainers/dotnet AS build
WORKDIR /src

# 1. Copy the entire repository first
COPY . .

# 2. Inject token straight into NuGet.config file and restore cleanly
RUN --mount=type=secret,id=aws_token \
    CODEARTIFACT_TOKEN=$(cat /run/secrets/aws_token) && \
    sed -i "s|%CODEARTIFACT_TOKEN%|$CODEARTIFACT_TOKEN|g" NuGet.config && \
    dotnet restore TraineeManagementApi.csproj --configfile NuGet.config

# 3. Publish ONLY the API project file explicitly
RUN dotnet publish TraineeManagementApi.csproj \
    -c Release \
    -o /App/out \
    --no-restore

# Build runtime image
FROM docker-registry-002.zeuslearning.com/zeuslearning/vscode/devcontainers/dotnet
WORKDIR /App
COPY --from=build /App/out .
ENTRYPOINT ["dotnet", "TraineeManagementApi.dll"]
