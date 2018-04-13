using System;

namespace LocationHelper
{
    public class Location
    {
        public Location(double lat, double lon, double? altitude = null, double? speed = null, double? heading = null, double? accuracy = null, double? verticalAccuracy = null)
        {
            Latitude = lat;
            Longitude = lon;
            Altitude = altitude;
            Speed = speed;
            Heading = heading;
            HorizontalAccuracy = accuracy;
            VerticalAccuracy = verticalAccuracy;
        }
        public double Longitude { get;  }
        public double Latitude { get; }
        public double? Altitude { get; }
        public double? Speed { get; }
        public double? Heading { get; }
        public double? HorizontalAccuracy { get; }
        public double? VerticalAccuracy { get; }
    }
}
