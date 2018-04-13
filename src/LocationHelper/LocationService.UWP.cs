#if NETFX_CORE
using System;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace LocationHelper
{
    public partial class LocationService
    {
        private Geolocator _locator;
        private TaskCompletionSource<object> _tcs;

        private Task StartAsync_Impl()
        {
            if (_tcs != null)
                return _tcs.Task; //Already started

            var tcs = _tcs = new TaskCompletionSource<object>();
            _locator = new Geolocator()
            {
                DesiredAccuracy = PositionAccuracy.High,
                MovementThreshold = 0,
                ReportInterval = 1000,
            };
            if (_locator.LocationStatus == PositionStatus.NotAvailable)
            {
                tcs.TrySetException(new NotSupportedException("Location services is not available on this version of Windows"));
            }
            else
            {
                Task.Run(async () =>
                {
                    Geoposition position = null;
                    try
                    {
                        //Trigger consent
                        position = await _locator.GetGeopositionAsync(TimeSpan.FromHours(1), TimeSpan.FromMilliseconds(1));
                        tcs.TrySetResult(null);
                        _locator.PositionChanged += Locator_PositionChanged;
                        if (position != null)
                        {
                            var c = position.Coordinate;
                            RaiseLocationUpdated(c.Point.Position.Latitude, c.Point.Position.Longitude, c.Point.Position.Altitude, c.Speed, c.Heading, c.Accuracy, c.AltitudeAccuracy);
                        }
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        _locator = null;
                        _tcs = null;
                        System.Diagnostics.Debug.WriteLine("GPS Failed to start due to location access being restricted");
                        tcs.TrySetException(ex);
                    }
                    catch (System.Exception ex)
                    {
                        _locator = null;
                        _tcs = null;
                        System.Diagnostics.Debug.WriteLine("GPS Error: " + ex.Message);
                        tcs.TrySetException(ex);
                    }
                });
            }
            return tcs.Task;
        }

        private Task StopAsync_Impl()
        {
            if (_tcs != null)
            {
                _tcs.TrySetCanceled();
                _tcs = null;
            }
            if (_locator != null)
            {
                _locator.PositionChanged -= Locator_PositionChanged;
                _locator = null;
            }
            return Task.CompletedTask;
        }

        private void Locator_PositionChanged(object sender, PositionChangedEventArgs args)
        {
            var c = args.Position.Coordinate;
            RaiseLocationUpdated(c.Point.Position.Latitude, c.Point.Position.Longitude, c.Point.Position.Altitude, c.Speed, c.Heading, c.Accuracy, c.AltitudeAccuracy);
        }
    }
}

#endif