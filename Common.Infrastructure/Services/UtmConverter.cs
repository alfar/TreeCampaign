namespace Common.Infrastructure.Services;

internal static class UtmConverter
{
    private const double SemiMajorAxis = 6378137.0;
    private const double Flattening = 1 / 298.257223563;
    private const double ScaleFactor = 0.9996;

    public static (decimal Latitude, decimal Longitude) ToLatLon(double easting, double northing, int zone, bool isNorthern)
    {
        var e = Math.Sqrt(Flattening * (2 - Flattening));
        var e1Sq = e * e / (1 - e * e);
        var x = easting - 500000.0;
        var y = isNorthern ? northing : northing - 10000000.0;

        var m = y / ScaleFactor;
        var mu = m / (SemiMajorAxis * (1 - e * e / 4 - 3 * Math.Pow(e, 4) / 64 - 5 * Math.Pow(e, 6) / 256));

        var e1 = (1 - Math.Sqrt(1 - e * e)) / (1 + Math.Sqrt(1 - e * e));
        var j1 = 3 * e1 / 2 - 27 * Math.Pow(e1, 3) / 32;
        var j2 = 21 * Math.Pow(e1, 2) / 16 - 55 * Math.Pow(e1, 4) / 32;
        var j3 = 151 * Math.Pow(e1, 3) / 96;
        var j4 = 1097 * Math.Pow(e1, 4) / 512;

        var footprintLatitude = mu
            + j1 * Math.Sin(2 * mu)
            + j2 * Math.Sin(4 * mu)
            + j3 * Math.Sin(6 * mu)
            + j4 * Math.Sin(8 * mu);

        var c1 = e1Sq * Math.Pow(Math.Cos(footprintLatitude), 2);
        var t1 = Math.Pow(Math.Tan(footprintLatitude), 2);
        var r1 = SemiMajorAxis * (1 - e * e) / Math.Pow(1 - e * e * Math.Pow(Math.Sin(footprintLatitude), 2), 1.5);
        var n1 = SemiMajorAxis / Math.Sqrt(1 - e * e * Math.Pow(Math.Sin(footprintLatitude), 2));
        var d = x / (n1 * ScaleFactor);

        var q1 = n1 * Math.Tan(footprintLatitude) / r1;
        var q2 = d * d / 2;
        var q3 = (5 + 3 * t1 + 10 * c1 - 4 * c1 * c1 - 9 * e1Sq) * Math.Pow(d, 4) / 24;
        var q4 = (61 + 90 * t1 + 298 * c1 + 45 * t1 * t1 - 252 * e1Sq - 3 * c1 * c1) * Math.Pow(d, 6) / 720;
        var latitudeRad = footprintLatitude - q1 * (q2 - q3 + q4);

        var q5 = d;
        var q6 = (1 + 2 * t1 + c1) * Math.Pow(d, 3) / 6;
        var q7 = (5 - 2 * c1 + 28 * t1 - 3 * c1 * c1 + 8 * e1Sq + 24 * t1 * t1) * Math.Pow(d, 5) / 120;
        var longitudeRad = (q5 - q6 + q7) / Math.Cos(footprintLatitude);

        var zoneCentralMeridian = 6 * zone - 183.0;
        var latitude = RadToDeg(latitudeRad);
        var longitude = RadToDeg(longitudeRad) + zoneCentralMeridian;

        return ((decimal)latitude, (decimal)longitude);
    }

    private static double RadToDeg(double rad) => rad * (180.0 / Math.PI);
}
