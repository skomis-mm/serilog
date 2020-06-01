#!/bin/bash
dotnet restore

for path in test/*.PerformanceTests/*.csproj; do
    dotnet test -f netcoreapp2.1 -c Release ${path}
    dotnet test -f netcoreapp3.1 -c Release ${path}
done
