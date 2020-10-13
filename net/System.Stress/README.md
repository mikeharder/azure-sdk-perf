# Example
```
> dotnet run -f netcoreapp3.1 sendreceive

=== Final Metrics ===
Start:                   10/1/2020 5:31:55 PM
Current:                 10/1/2020 5:32:06 PM
End:                     10/1/2020 5:32:05 PM
TotalProcessorTime:      00:00:00.1875000
Current Memory:          730.91 KB
Average Memory:          567.71 KB
Peak Memory:             730.91 KB
GC Gen 0 Collections:    0
GC Gen 1 Collections:    0
GC Gen 2 Collections:    0
Total Exceptions:        16
ReceiveException:        9
SendException:           7
Receives:                315
Sends:                   315
Unprocessed:             0

=== Exceptions ===
System.Stress.SendReceiveTest+SendException: 0.006969725250718056
System.Stress.SendReceiveTest+SendException: 0.00832290389031307
System.Stress.SendReceiveTest+SendException: 2.9501970871119746E-05
System.Stress.SendReceiveTest+ReceiveException: 0.0006806422028134773
System.Stress.SendReceiveTest+ReceiveException: 0.014633518650491497
System.Stress.SendReceiveTest+ReceiveException: 0.013826194225729534
System.Stress.SendReceiveTest+ReceiveException: 0.016874472618510234
System.Stress.SendReceiveTest+SendException: 0.0021216082396552007
System.Stress.SendReceiveTest+ReceiveException: 0.008734948471530782
System.Stress.SendReceiveTest+ReceiveException: 0.016180066399360107
System.Stress.SendReceiveTest+ReceiveException: 0.00035485299320651823
System.Stress.SendReceiveTest+SendException: 0.0021563363271562084
System.Stress.SendReceiveTest+SendException: 0.00474277045798617
System.Stress.SendReceiveTest+SendException: 0.009259710092684118
System.Stress.SendReceiveTest+ReceiveException: 0.016751021154574594
System.Stress.SendReceiveTest+ReceiveException: 0.0032293889686602116
```
