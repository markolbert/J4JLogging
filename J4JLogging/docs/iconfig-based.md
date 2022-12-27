# The Configuration File

Using `Serilog`'s `IConfiguration`-based add-on to automagically configure the `Serilog` logger underlying `J4JLogger` requires you be familiar with `Serilog`'s JSON syntax. You should consult the [github site for details](https://github.com/serilog/serilog-settings-configuration), but it's pretty simple:

```json
  "Serilog": {
    "MinimumLevel": "Information",
    "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.File", "Serilog.Sinks.Debug" ],
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "{Timestamp:HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {CallingContext} {Exception}{NewLine}"
        }
      },
      {
        "Name": "Debug",
        "Args": {
          "outputTemplate": "{Timestamp:HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {CallingContext} {Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "log.txt",
          "rollingInterval": "Day",
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {CallingContext} {Exception}{NewLine}"
        }
      }
    ]
  }
```

If you want the calling context information (e.g., calling method, source code file and source code line number) to appear you need to include {CallingContext} somewhere in the output template. Similarly, if you want to use the SMS sink provided in `J4JLogger` you need to include {SendToSms}.

I am looking into finding a way to have those inclusions occur automagically whenever you use `J4JLogger`. Stay tuned!
