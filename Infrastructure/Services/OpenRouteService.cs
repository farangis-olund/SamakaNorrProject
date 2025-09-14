using Infrastructure.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Services;

public class OpenRouteService
{
	private readonly HttpClient _http;
	private readonly string _apiKey = null!;

	public OpenRouteService(HttpClient http, IConfiguration config)
	{
		_http = http;
        _apiKey = config["OpenRouteServiceApiKey"];
    }

	public async Task<(double DistanceKm, TimeSpan Duration)?> GetDrivingInfoAsync(string origin, string destination)
	{
		if (!LocationData.Coordinates.ContainsKey(origin) ||
			!LocationData.Coordinates.ContainsKey(destination))
			return null;

		var (lat1, lon1) = LocationData.Coordinates[origin];
		var (lat2, lon2) = LocationData.Coordinates[destination];
		Console.WriteLine("API Key: " + _apiKey);
		var url = Environment.GetEnvironmentVariable("OpenRouteServiceApiKey");



		var response = await _http.GetAsync(url);
		if (!response.IsSuccessStatusCode) return null;

		var json = await response.Content.ReadAsStringAsync();

		using var doc = JsonDocument.Parse(json);
		var summary = doc.RootElement
			.GetProperty("features")[0]
			.GetProperty("properties")
			.GetProperty("summary");

		double distanceKm = summary.GetProperty("distance").GetDouble() / 1000.0; // meters → km
		double durationSec = summary.GetProperty("duration").GetDouble();
		TimeSpan duration = TimeSpan.FromSeconds(durationSec);

		return (distanceKm, duration);
	}
}
