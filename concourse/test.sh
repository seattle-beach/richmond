#!/bin/bash -e

project_root=$1

cd $project_root
dotnet restore
cd test
dotnet test
