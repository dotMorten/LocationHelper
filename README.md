# LocationHelper
A .NET Standard multi-targeted Location Services API for handling location updates across multiple platforms.
Currently supports getting location on UWP, .NET 4.6.2+, Android and iOS.


### Usage:

```cs
if (LocationService.IsLocationServiceSupported) //This returns false for platforms that are not implemented
{
    var svc = new LocationService();
    svc.LocationUpdated += LocationUpdated;
    await svc.StartAsync();
}
```

Make sure location capabilities are enabled for your application, and on Android ensure you requested location access first.
