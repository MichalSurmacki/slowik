#!/bin/bash

set -e
# run_cmd="dotnet /app/out/Api.dll"

export PATH="$PATH:/root/.dotnet/tools"

>&2 echo "Running migrations"

until dotnet ef database update -s ./Api/; do
    >&2 echo "Migrations executing"
    sleep 1
done

>&2 echo "DB Migrations complete, starting app."
# >&2 echo "Running': $run_cmd"
dotnet /app/out/Api.dll --urls=http://0.0.0.0:80