#if __IOS__
using CoreLocation;
using Foundation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LocationHelper
{
    public partial class LocationService
    {
        private CLLocationManager _locationManager;
        private TaskCompletionSource<object> _tcs;

        private Task StartAsync_Impl()
        {
            if (_tcs != null)
                return _tcs.Task;
            _tcs = new TaskCompletionSource<object>();
            new NSObject().InvokeOnMainThread(() =>
            {
                _locationManager = new CLLocationManager()
                {
                    DistanceFilter = 1
                };
                _locationManager.UpdatedLocation += LocationManager_UpdatedLocation;

                if (CLLocationManager.Status == CLAuthorizationStatus.NotDetermined)
                {
                    // Request permission to track location and listen for changes to authorization
                    _locationManager.AuthorizationChanged += LocationManager_AuthorizationChanged;
                    _locationManager.RequestWhenInUseAuthorization();
                }
                if (CLLocationManager.Status == CLAuthorizationStatus.AuthorizedWhenInUse ||
                   CLLocationManager.Status == CLAuthorizationStatus.AuthorizedAlways ||
                   CLLocationManager.Status == CLAuthorizationStatus.Authorized)
                {
                    _locationManager.StartUpdatingLocation();
                    _tcs.TrySetResult(null);
                }

            });
            return _tcs.Task;
        }

        private Task StopAsync_Impl()
        {
            if(_locationManager != null)
            {
                if(_tcs != null)
                {
                    _tcs.TrySetCanceled();
                }
                _locationManager.UpdatedLocation -= LocationManager_UpdatedLocation;
                _locationManager.AuthorizationChanged -= LocationManager_AuthorizationChanged;
                _locationManager = null;
            }
            _tcs = null;
            return Task.CompletedTask;
        }

        private void LocationManager_AuthorizationChanged(object sender, CLAuthorizationChangedEventArgs e)
        {
            _locationManager.AuthorizationChanged -= LocationManager_AuthorizationChanged;
            if (CLLocationManager.Status == CLAuthorizationStatus.AuthorizedWhenInUse ||
               CLLocationManager.Status == CLAuthorizationStatus.AuthorizedAlways ||
               CLLocationManager.Status == CLAuthorizationStatus.Authorized)
            {
                _locationManager.StartUpdatingLocation();
                _tcs.TrySetResult(null);
            }
            else
            {
                _tcs.TrySetException(new UnauthorizedAccessException("User denied location access"));
            }
        }

        private void LocationManager_UpdatedLocation(object sender, CLLocationUpdatedEventArgs e)
        {
            var l = e.NewLocation;
            RaiseLocationUpdated(l.Coordinate.Latitude, l.Coordinate.Latitude,
                l.VerticalAccuracy < 0 ? null : (double?)l.Altitude,
               l.Speed < 0 ? null : (double?)l.Speed, 
               l.Course < 0 ? null : (double?)l.Course,
               l.HorizontalAccuracy < 0 ? null : (double?)l.HorizontalAccuracy,
               l.VerticalAccuracy < 0 ? null : (double?)l.VerticalAccuracy);
        }
    }
}

#endif