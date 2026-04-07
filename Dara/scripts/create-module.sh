#!/bin/bash

MODULE_NAME=$1
BASE_DIR="src/Modules/$MODULE_NAME"
PROJECT_PREFIX="Dara.Modules.$MODULE_NAME"

echo "creating module $1 at dir $BASE_DIR with prefix $PROJECT_PREFIX"
dotnet new classlib -n "$PROJECT_PREFIX.Domain" -o "$BASE_DIR/Domain"
dotnet new classlib -n "$PROJECT_PREFIX.Application" -o "$BASE_DIR/Application"
dotnet new classlib -n "$PROJECT_PREFIX.Infrastructure" -o "$BASE_DIR/Infrastructure"
dotnet new console -n "$PROJECT_PREFIX.Tests" -o "$BASE_DIR/Tests"

echo "Adding reference app -> domain, infra -> domain, tests -> app,domain,infra"
dotnet add "$BASE_DIR/Application/$PROJECT_PREFIX.Application.csproj" reference "$BASE_DIR/Domain/$PROJECT_PREFIX.Domain.csproj"
dotnet add "$BASE_DIR/Infrastructure/$PROJECT_PREFIX.Infrastructure.csproj" reference "$BASE_DIR/Domain/$PROJECT_PREFIX.Domain.csproj"

dotnet add "$BASE_DIR/Tests/$PROJECT_PREFIX.Tests.csproj" reference "$BASE_DIR/Domain/$PROJECT_PREFIX.Domain.csproj"
dotnet add "$BASE_DIR/Tests/$PROJECT_PREFIX.Tests.csproj" reference "$BASE_DIR/Application/$PROJECT_PREFIX.Application.csproj"
dotnet add "$BASE_DIR/Tests/$PROJECT_PREFIX.Tests.csproj" reference "$BASE_DIR/Infrastructure/$PROJECT_PREFIX.Infrastructure.csproj"
