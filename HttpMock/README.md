## Example
### Run Perf Test against Live Storage
1. Create storage account
2. Set `STORAGE_CONNECTION_STRING` env var
3. `cd ../net/Azure.Storage.Blobs.PerfStress`

```
> dotnet run -c release -f netcoreapp3.1 -- download

=== Results ===
Completed 495 operations in a weighted-average of 10.02s (49.42 ops/s, 0.020 s/op)
```

### Start HttpMock Server
1. Open second command prompt

```
> dotnet run -c release -- -s storageblob --dots

=== Startup ===
Hosting environment: Development
Content root path: C:\Git\perf\HttpMock
Now listening on: http://0.0.0.0:7777
Now listening on: https://0.0.0.0:7778
Application started. Press Ctrl+C to shut down.
```

2. Arguments
   1. Argument `-s storageblob` tells the server which headers it needs to respect (different for each service).
   2. Argument `--dots` tells the server to print `*` for each cache miss and `.` for each cache hit.  This is useful when debugging but should be disabled when actually measuring perf.


## Run Perf Test against HttpMock Server
1. Go back to first command prompt

```
> dotnet run -c release -f netcoreapp3.1 -- download --insecure --host localhost --port 7778

=== Results ===
Completed 20626 operations in a weighted-average of 10.03s (2,055.50 ops/s, 0.000 s/op)
```

2. Arguments
   1. Argument `--insecure` tells client to allow untrusted (self-signed) SSL certs.
   2. Arguments `--host` and `--port` tells where to redirect requests.  This sample uses `localhost` but it could be a different machine.
3. Throughtput is much higher since responses are returned from the local cache instead of requiring a roundtrip to the live service.
4. HttpMock server will print `***...`.  This means the first 3 requests (create container, upload blob, download blob) were cache misses and sent to live storage.  The next 3 requests (all download blob) were cache hits.
