#!/bin/bash -e

test_directory=$1

cd $test_directory
dotnet restore
dotnet test
