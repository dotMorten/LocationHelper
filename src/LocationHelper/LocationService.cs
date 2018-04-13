using System;
using System.Threading.Tasks;

namespace LocationHelper
{
    public partial class LocationService
    {
        /// <summary>
        /// Gets a value indicating whether this platform can support a location service.
        /// </summary>
        public static bool IsLocationServiceSupported =>
#if NETSTANDARD2_0
            false;
#else
            true;
#endif

        /// <summary>
        /// Starts the location service.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="PlatformNotSupportedException">Thrown if <see cref="IsLocationServiceSupported"/> is <c>False</c>.</exception>
        public Task StartAsync()
        {
#if NETSTANDARD2_0
            // There are no .NET Standard 2.0 APIs for location services
            // However that's ok since the .NET Standard Assembly will only be used if the NuGet is
            // not referenced by Android, iOS, UWP or .NET Framework project heads
            throw new PlatformNotSupportedException(); 
#else
            return StartAsync_Impl();
#endif
        }

        /// <summary>
        /// Stops the location service.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="PlatformNotSupportedException">Thrown if <see cref="IsLocationServiceSupported"/> is <c>False</c>.</exception>
        public Task StopAsync()
        {
#if NETSTANDARD2_0
            return Task.CompletedTask;
#else
            return StopAsync_Impl();
#endif
        }

        //TODO: Add StopAsync

        private void RaiseLocationUpdated(double lat, double lon, double? altitude, double? speed, double? heading, double? accuracy, double? verticalAccuracy)
        {
            LocationUpdated?.Invoke(this, new Location(lat, lon, altitude, speed, heading, accuracy, verticalAccuracy));
        }

        /// <summary>
        /// Raised when the location has changed
        /// </summary>
        public event EventHandler<Location> LocationUpdated;
    }
}
