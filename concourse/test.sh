#!/bin/bash -e

project_root=$1

cd $project_root
cd src
echo "Restoring src..."
dotnet restore
cd ../test-backend
echo "Restoring tests..."
dotnet restore
echo "Running tests..."
dotnet test
