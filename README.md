# LocationHelper
A Multi-target Location Services API for handling location updates across multiple platforms


### Usage:

```cs
if (LocationService.IsLocationServiceSupported)
{
    var svc = new LocationService();
    svc.LocationUpdated += LocationUpdated;
    await svc.StartAsync();
}
```

Make sure location capabilities are enabled for your application, and on Android ensure you requested location access first.