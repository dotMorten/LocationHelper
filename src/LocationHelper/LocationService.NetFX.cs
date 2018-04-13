#if NETFX
using System.Device.Location;
using System.Threading.Tasks;

namespace LocationHelper
{
    public partial class LocationService
    {
        private GeoCoordinateWatcher watcher;

        private Task StartAsync_Impl()
        {
            if (watcher == null)
            {
                watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
                watcher.PositionChanged += Watcher_PositionChanged;
                watcher.Start();
            }
            return Task.CompletedTask;
        }

        private Task StopAsync_Impl()
        {
            if (watcher != null)
            {
                watcher.PositionChanged -= Watcher_PositionChanged;
                watcher.Stop();
                watcher = null;
            }
            return Task.CompletedTask;
        }

        private void Watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            var l = e.Position.Location;
            RaiseLocationUpdated(l.Latitude, l.Longitude, NanToNull(l.Altitude), NanToNull(l.Speed), NanToNull(l.Course), NanToNull(l.HorizontalAccuracy), NanToNull(l.VerticalAccuracy));
        }
        private static double? NanToNull(double v) => double.IsNaN(v) ? null : (double?)v;
    }
}
#endif