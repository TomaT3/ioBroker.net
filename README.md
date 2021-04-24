# ioBroker.net
Enables easy communication with ioBroker for .NET Projects

## Preconditions

- Installed [socket.io](https://github.com/ioBroker/ioBroker.socketio/) adapter in ioBroker

## Usage

Install NugetPackage ioBroker.net from [nuget.org](https://www.nuget.org/packages/ioBroker.net/)

### Create connection to socket.io adapter in ioBroker
```c#
using using ioBroker.net;
...
var ioBroker = new IoBrokerDotNet("http://iobroker:8084");
await ioBroker.ConnectAsync(TimeSpan.FromSeconds(5));
```

### Read value
```c#
var id = "deconz.0.Lights.13.level"; // the object id you want to read
var lightLevel = await ioBroker.GetStateAsync<int>(id, TimeSpan.FromSeconds(5));
```

### Write value
```c#
var id = "deconz.0.Lights.13.level"; // the object id you want to set
await ioBroker.SetStateAsync<int>(id, 55);
```

### Subscribe for changes
```c#
var id = "deconz.0.Lights.13.level"; // the object id you want to get notifications if value changes
await ioBroker.SubscribeStateAsync<int>(id, LightLevelChangedHandler);

...

private static void LightLevelChangedHandler(int lightLevel)
{
    // do whatever you want when you receive changes for the subscribed value
    Console.WriteLine($"Light level has changed: {lightLevel}");
}
```
