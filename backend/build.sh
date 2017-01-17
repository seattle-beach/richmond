#!/bin/bash -e

source_directory=$1
output_destination=$(pwd)/$2

cd $source_directory
dotnet restore
dotnet publish -o $output_destination
