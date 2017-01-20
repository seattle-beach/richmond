#!/bin/bash -e
./fly -t lite set-pipeline -p richmond -c main.yml --var "CF_PASSWORD=$CF_PASSWORD" --var "CF_USERNAME=$CF_USERNAME" --var "TRACKER_API_TOKEN=$TRACKER_API_TOKEN"
