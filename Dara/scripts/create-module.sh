#!/bin/bash

MODULE_NAME=$1
BASE_DIR="src/Modules/$MODULE_NAME"
BB_DIR="src/BuildingBlocks"

PROJECT_PREFIX="Dara.Modules.$MODULE_NAME"
BB_PREFIX="Dara.BuildingBlocks"

echo "creating module $1 at dir $BASE_DIR with prefix $PROJECT_PREFIX"

dotnet new classlib -n "$PROJECT_PREFIX.Domain" -o "$BASE_DIR/Domain"
$MOD_DOM = "$BASE_DIR/Domain/$PROJECT_PREFIX.Domain.csproj"
$BB_DOM = "$BB_DIR/Domain/$BB_PREFIX.Domain.csproj"

dotnet new classlib -n "$PROJECT_PREFIX.Application" -o "$BASE_DIR/Application"
$MOD_APP = "$BASE_DIR/Application/$PROJECT_PREFIX.Application.csproj"
$BB_APP = "$BB_DIR/Application/$BB_PREFIX.Application.csproj"

dotnet new classlib -n "$PROJECT_PREFIX.Infrastructure" -o "$BASE_DIR/Infrastructure"
$MOD_INFRA = "$BASE_DIR/Infrastructure/$PROJECT_PREFIX.Infrastructure.csproj"
$BB_INFRA = "$BB_DIR/Infrastructure/$BB_PREFIX.Infrastructure.csproj"

dotnet new classlib -n "$PROJECT_PREFIX.Contracts" -o "$BASE_DIR/Contracts"
$MOD_CONT = "$BASE_DIR/Contracts/$PROJECT_PREFIX.Contracts.csproj"
$BB_CONT = "$BB_DIR/Contracts/$BB_PREFIX.Contracts.csproj"

dotnet new classlib -n "$PROJECT_PREFIX.Configuration" -o "$BASE_DIR/Configuration"
$MOD_CONF = "$BASE_DIR/Configuration/$PROJECT_PREFIX.Configuration.csproj"
$BB_CONF = "$BB_DIR/Configuration/$BB_PREFIX.Configuration.csproj"

echo "Adding domain layer references"
dotnet add $MOD_DOM references $BB_DOM

echo "Adding application layer references"
dotnet add $MOD_APP references $BB_APP
dotnet add $MOD_APP references $MOD_DOM
dotnet add $MOD_APP references $MOD_CONT

echo "Adding infrastructure layer references"
dotnet add $MOD_INFRA references $BB_INFRA
dotnet add $MOD_INFRA references $MOD_DOM
dotnet add $MOD_INFRA references $MOD_CONT

echo "Adding contract layer references"
dotnet add $MOD_CONT references $BB_CONT

echo "Adding configuration layer references"
dotnet add $MOD_CONF references $BB_CONF
dotnet add $MOD_CONF references $MOD_APP
dotnet add $MOD_CONF references $MOD_INFRA
