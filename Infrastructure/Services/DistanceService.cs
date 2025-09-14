
using Infrastructure.Models;

namespace Infrastructure.Services;

/// <summary>
/// Service to calculate distances and approximate travel times between locations.
/// Uses Haversine formula for "as-the-crow-flies" distance.
/// </summary>
public class DistanceService
{
	/// <summary>
	/// Calculates the great-circle distance between two places in kilometers
	/// using the Haversine formula.
	/// </summary>
	/// <param name="origin">Origin location name (must exist in LocationData)</param>
	/// <param name="destination">Destination location name (must exist in LocationData)</param>
	/// <returns>Distance in kilometers (double)</returns>
	/// <exception cref="ArgumentException"></exception>
	public double Haversine(string origin, string destination)
	{
		if (!LocationData.Coordinates.ContainsKey(origin) ||
			!LocationData.Coordinates.ContainsKey(destination))
		{
			throw new ArgumentException("One or both locations not found in dictionary");
		}

		var (lat1, lon1) = LocationData.Coordinates[origin];
		var (lat2, lon2) = LocationData.Coordinates[destination];

		const double R = 6371.0; // Earth radius in kilometers
		var φ1 = DegreesToRadians(lat1);
		var φ2 = DegreesToRadians(lat2);
		var Δφ = DegreesToRadians(lat2 - lat1);
		var Δλ = DegreesToRadians(lon2 - lon1);

		var a = Math.Sin(Δφ / 2) * Math.Sin(Δφ / 2) +
				Math.Cos(φ1) * Math.Cos(φ2) *
				Math.Sin(Δλ / 2) * Math.Sin(Δλ / 2);
		var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

		return R * c; // Distance in kilometers
	}

	/// <summary>
	/// Estimates travel time between two places assuming an average speed.
	/// Default average speed is 80 km/h (rural roads).
	/// </summary>
	/// <param name="origin">Origin location name</param>
	/// <param name="destination">Destination location name</param>
	/// <param name="averageSpeedKmh">Average travel speed in km/h</param>
	/// <returns>TimeSpan with estimated travel time</returns>
	public TimeSpan EstimateTravelTime(string origin, string destination, double averageSpeedKmh = 80.0)
	{
		var distanceKm = Haversine(origin, destination);
		var hours = distanceKm / averageSpeedKmh;
		return TimeSpan.FromHours(hours);
	}

	/// <summary>
	/// Converts degrees to radians.
	/// </summary>
	private double DegreesToRadians(double degrees) => degrees * Math.PI / 180.0;
}
