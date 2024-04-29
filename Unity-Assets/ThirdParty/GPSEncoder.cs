// Copyright 2013 MichaelTaylor3D
// www.michaeltaylor3d.com

using UnityEngine;

public sealed class GPSEncoder
{

    /////////////////////////////////////////////////
    //////-------------Public API--------------//////
    /////////////////////////////////////////////////

    /// <summary>
    /// Convert UCS (X,Y,Z) coordinates to GPS (Lat, Lon) coordinates
    /// </summary>
    /// <returns>
    /// Returns Vector2 containing Latitude and Longitude
    /// </returns>
    /// <param name='position'>
    /// (X,Y,Z) Position Parameter
    /// </param>
    public static Vector2 UCSToGPS(Vector3 position)
    {
        return GetInstance().ConvertUCStoGPS(position);
    }

    /// <summary>
    /// Convert GPS (Lat, Lon) coordinates to UCS (X,Y,Z) coordinates
    /// </summary>
    /// <returns>
    /// Returns a Vector3 containing (X, Y, Z)
    /// </returns>
    /// <param name='gps'>
    /// (Lat, Lon) as Vector2
    /// </param>
    public static Vector3 GPSToUCS(Vector2 gps)
    {
        return GetInstance().ConvertGPStoUCS(gps);
    }

    /// <summary>
    /// Convert GPS (Lat, Lon) coordinates to UCS (X,Y,Z) coordinates
    /// </summary>
    /// <returns>
    /// Returns a Vector3 containing (X, Y, Z)
    /// </returns>
    public static Vector3 GPSToUCS(double latitude, double longitude)
    {
        return GetInstance().ConvertGPStoUCS(new Vector2((float)latitude, (float)longitude));
    }

    /// <summary>
    /// Change the relative GPS offset (Lat, Lon), Default (0,0), 
    /// used to bring a local area to (0,0,0) in UCS coordinate system
    /// </summary>
    /// <param name='localOrigin'>
    /// Reference point.
    /// </param>
    public static void SetLocalOrigin(Vector2 localOrigin)
    {
        GetInstance()._localOrigin = localOrigin;
    }

    /////////////////////////////////////////////////
    //////---------Instance Members------------//////
    /////////////////////////////////////////////////

    #region Singleton
    private static GPSEncoder _singleton;

    private GPSEncoder()
    {

    }

    private static GPSEncoder GetInstance()
    {
        if (_singleton == null)
        {
            _singleton = new GPSEncoder();
        }
        return _singleton;
    }
    #endregion

    #region Instance Variables
    private Vector2 _localOrigin = Vector2.zero;
    private double _LatOrigin { get { return _localOrigin.x; } }
    private double _LonOrigin { get { return _localOrigin.y; } }

    private double metersPerLat;
    private double metersPerLon;
    #endregion

    #region Instance Functions
    private void FindMetersPerLat(double lat) // Compute lengths of degrees
    {
        double m1 = 111132.92;    // latitude calculation term 1
        double m2 = -559.82;        // latitude calculation term 2
        double m3 = 1.175;      // latitude calculation term 3
        double m4 = -0.0023;        // latitude calculation term 4
        double p1 = 111412.84;    // longitude calculation term 1
        double p2 = -93.5;      // longitude calculation term 2
        double p3 = 0.118;      // longitude calculation term 3

        lat = lat * Mathf.Deg2Rad;

        // Calculate the length of a degree of latitude and longitude in meters
        metersPerLat = m1 + (m2 * Mathf.Cos(2 * (float)lat)) + (m3 * Mathf.Cos(4 * (float)lat)) + (m4 * Mathf.Cos(6 * (float)lat));
        metersPerLat = metersPerLat * 10;
        metersPerLon = (p1 * Mathf.Cos((float)lat)) + (p2 * Mathf.Cos(3 * (float)lat)) + (p3 * Mathf.Cos(5 * (float)lat));
        metersPerLon = metersPerLon * 10;
    }

    private Vector3 ConvertGPStoUCS(Vector2 gps)
    {
        FindMetersPerLat(_LatOrigin);
        double zPosition = metersPerLat * (gps.x - _LatOrigin); // Calc current lat
        double xPosition = metersPerLon * (gps.y - _LonOrigin); // Calc current lat
        return new Vector3((float)xPosition, 0, (float)zPosition);
    }

    private Vector2 ConvertUCStoGPS(Vector3 position)
    {
        FindMetersPerLat(_LatOrigin);
        Vector2 geoLocation = new Vector2(0, 0);
        geoLocation.x = (float)(_LatOrigin + (position.z) / metersPerLat); // Cast to float
        geoLocation.y = (float)(_LonOrigin + (position.x) / metersPerLon); // Cast to float
        return geoLocation;
    }
    #endregion
}
