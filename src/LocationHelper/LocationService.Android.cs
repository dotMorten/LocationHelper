#if __ANDROID__
using Android.App;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LocationHelper
{
    public partial class LocationService
    {
        private AndroidLocationService _service;

        private Task StartAsync_Impl()
        {
            if (_service == null)
            {
                _service = new AndroidLocationService();
                _service.OnLocationUpdated += Service_OnLocationUpdated;
            }
            return Task.CompletedTask;
        }

        private Task StopAsync_Impl()
        {
            if (_service != null)
            {
                _service.OnLocationUpdated -= Service_OnLocationUpdated;
                _service.Dispose();
                _service = null;
            }
            return Task.CompletedTask;
        }

        private void Service_OnLocationUpdated(object sender, Android.Locations.Location e)
        {
            RaiseLocationUpdated(e.Latitude, e.Longitude, e.HasAltitude ? (double?)e.Altitude : null, e.HasSpeed ? (double?)e.Speed : null,
                e.HasBearing ? (double?)e.Bearing : null, e.HasAccuracy ? (double?)e.Accuracy : null, null);
        }

        private sealed class AndroidLocationService : Java.Lang.Object, ILocationListener
        {
            private LocationManager _locationManager;

            public AndroidLocationService()
            {
                _locationManager = Application.Context.GetSystemService(Android.Content.Context.LocationService) as LocationManager;
                if (_locationManager == null)
                    throw new Exception("Location Services are unavailable");
                bool ok = false;
                if (_locationManager.IsProviderEnabled(LocationManager.GpsProvider))
                {
                    _locationManager.RequestLocationUpdates(LocationManager.GpsProvider, 1000, 1f, this, Looper.MainLooper);
                    ok = true;
                }
                if (_locationManager.IsProviderEnabled(LocationManager.NetworkProvider))
                {
                    _locationManager.RequestLocationUpdates(LocationManager.NetworkProvider, 1000, 1f, this, Looper.MainLooper);
                    ok = true;
                }
                if (!ok)
                {
                    throw new InvalidOperationException("Location providers are not enabled");
                }
            }

            protected override void Dispose(bool disposing)
            {
                _locationManager.RemoveUpdates(this);
                base.Dispose(disposing);
            }

            public event EventHandler<Android.Locations.Location> OnLocationUpdated;

            void ILocationListener.OnLocationChanged(Android.Locations.Location location)
            {
                OnLocationUpdated?.Invoke(this, location);
            }

            void ILocationListener.OnProviderDisabled(string provider) { }

            void ILocationListener.OnProviderEnabled(string provider) { }

            void ILocationListener.OnStatusChanged(string provider, Availability status, Bundle extras) { }
        }
    }
}

#endif