#!/bin/bash

MODULE_NAME=$1
BASE_DIR="src/Server/Modules/$MODULE_NAME"
BB_DIR="src/Server/BuildingBlocks"

PROJECT_PREFIX="Dara.Server.Modules.$MODULE_NAME"
BB_PREFIX="Dara.Server.BuildingBlocks"

echo "creating module $1 at dir $BASE_DIR with prefix $PROJECT_PREFIX"

dotnet new classlib -n "$PROJECT_PREFIX.Domain" -o "$BASE_DIR/Domain"
MOD_DOM="$BASE_DIR/Domain/$PROJECT_PREFIX.Domain.csproj"
BB_DOM="$BB_DIR/Domain/$BB_PREFIX.Domain.csproj"

dotnet new classlib -n "$PROJECT_PREFIX.Application" -o "$BASE_DIR/Application"
MOD_APP="$BASE_DIR/Application/$PROJECT_PREFIX.Application.csproj"
BB_APP="$BB_DIR/Application/$BB_PREFIX.Application.csproj"

dotnet new classlib -n "$PROJECT_PREFIX.Infrastructure" -o "$BASE_DIR/Infrastructure"
MOD_INFRA="$BASE_DIR/Infrastructure/$PROJECT_PREFIX.Infrastructure.csproj"
BB_INFRA="$BB_DIR/Infrastructure/$BB_PREFIX.Infrastructure.csproj"

dotnet new classlib -n "$PROJECT_PREFIX.Integration" -o "$BASE_DIR/Integration"
MOD_INFRA="$BASE_DIR/Integration/$PROJECT_PREFIX.Integration.csproj"
BB_INFRA="$BB_DIR/Integration/$BB_PREFIX.Integration.csproj"

echo "Adding domain layer references"
dotnet add $MOD_DOM reference $BB_DOM

echo "Adding application layer references"
dotnet add $MOD_APP reference $BB_APP
dotnet add $MOD_APP reference $MOD_DOM

echo "Adding infrastructure layer references"
dotnet add $MOD_INFRA reference $BB_INFRA
dotnet add $MOD_INFRA reference $MOD_DOM

echo "Adding integration layer references"
dotnet add $MOD_INFRA reference $BB_INFRA
dotnet add $MOD_INFRA reference $MOD_DOM

echo "Done"
