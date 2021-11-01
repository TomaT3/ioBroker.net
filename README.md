# ioBroker.net
Enables easy communication with ioBroker for .NET Projects

[Release Notes](https://github.com/TomaT3/ioBroker.net/releases)

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
or use empty constructor and set ConnectionString before connecting
```c#
using using ioBroker.net;
...
var ioBroker = new IoBrokerDotNet();
ioBroker.ConnectionString = "http://ioBroker:8084";
await ioBroker.ConnectAsync(TimeSpan.FromSeconds(5));
```

### Read value
```c#
var id = "deconz.0.Lights.13.level"; // the object id you want to read
var result = await ioBroker.TryGetStateAsync<int>(id, TimeSpan.FromSeconds(5)); // you receive a GetStateResult<T>

// Check if reading was succesfull
if (result.Success)
{
    Console.WriteLine($"Success with value: {result.Value}");
}
else
{
    Console.WriteLine($"Error with Exception: {result.Error}");
}

```

### Write value
```c#
var id = "deconz.0.Lights.13.level"; // the object id you want to set
var result = await ioBroker.TrySetStateAsync<int>(id, 55); // you receive a SetStateResult<T>
// Check if writing was succesfull
if (result.Success)
{
    Console.WriteLine($"Success with writing value: {result.ValueToWrite}");
}
else
{
    Console.WriteLine($"Error while writing with Exception: {result.Error}");
}
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
