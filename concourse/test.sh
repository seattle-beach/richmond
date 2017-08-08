#!/bin/bash -ev

project_root=$1

cd $project_root
cd src
dotnet restore
cd test-backend
dotnet restore
dotnet test
