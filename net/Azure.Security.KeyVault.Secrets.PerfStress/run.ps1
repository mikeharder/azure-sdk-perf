#!/usr/bin/env pwsh

docker run -it --rm --network host -e STORAGE_CONNECTION_STRING azure-security-keyvault-secrets-perfstress-net @args
