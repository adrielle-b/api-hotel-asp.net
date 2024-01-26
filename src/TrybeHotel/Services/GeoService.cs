using System.Net.Http;
using TrybeHotel.Dto;
using TrybeHotel.Repository;

namespace TrybeHotel.Services
{
    public class GeoService : IGeoService
    {
         private readonly HttpClient _client;
        public GeoService(HttpClient client)
        {
            _client = client;
        }

        // 11. Desenvolva o endpoint GET /geo/status
        public async Task<object> GetGeoStatus()
        {
            var response = await _client.GetAsync("https://nominatim.openstreetmap.org/status.php?format=json");
            response.Headers.Add("Accept", "application/json");
            response.Headers.Add("User-Agent", "aspnet-user-agent");

            if (!response.IsSuccessStatusCode)
            {
                return default(Object);
            }
            var responseGeo = await response.Content.ReadFromJsonAsync<object>();
            return responseGeo;
        }
        
        // 12. Desenvolva o endpoint GET /geo/address
        public async Task<GeoDtoResponse> GetGeoLocation(GeoDto geoDto)
        {
            var response = await _client.GetAsync($"https://nominatim.openstreetmap.org/search?street={geoDto.Address}&city={geoDto.City}&country=Brazil&state={geoDto.State}&format=json&limit=1");
            response.Headers.Add("Accept", "application/json");
            response.Headers.Add("User-Agent", "aspnet-user-agent");

            if (!response.IsSuccessStatusCode)
            {
                return default(GeoDtoResponse);
            }
            var responseGeo = await response.Content.ReadFromJsonAsync<List<GeoDtoResponse>>();
            return new GeoDtoResponse 
            {
                lat = responseGeo[0].lat, 
                lon = responseGeo[0].lon 
            };
        }

        // 12. Desenvolva o endpoint GET /geo/address
        public async Task<List<GeoDtoHotelResponse>> GetHotelsByGeo(GeoDto geoDto, IHotelRepository repository)
        {
            var responseGeo = await GetGeoLocation(geoDto);
            var hotels = repository.GetHotels();
            var hotelsByGeo = new List<GeoDtoHotelResponse>();
            foreach (var hotel in hotels)
            {
                var location = await GetGeoLocation(new GeoDto { Address = hotel.address, City = hotel.cityName, State = hotel.state });
                var distance = CalculateDistance(responseGeo.lat, responseGeo.lon, location.lat, location.lon);
                hotelsByGeo.Add(new GeoDtoHotelResponse 
                {
                    HotelId = hotel.hotelId, 
                    Name = hotel.name, 
                    Address = hotel.address, 
                    CityName = hotel.cityName, 
                    State = hotel.state, 
                    Distance = distance 
                });
            }
            return hotelsByGeo.OrderBy(h => h.Distance).ToList();
        }

        public int CalculateDistance (string latitudeOrigin, string longitudeOrigin, string latitudeDestiny, string longitudeDestiny) {
            double latOrigin = double.Parse(latitudeOrigin.Replace('.',','));
            double lonOrigin = double.Parse(longitudeOrigin.Replace('.',','));
            double latDestiny = double.Parse(latitudeDestiny.Replace('.',','));
            double lonDestiny = double.Parse(longitudeDestiny.Replace('.',','));
            double R = 6371;
            double dLat = radiano(latDestiny - latOrigin);
            double dLon = radiano(lonDestiny - lonOrigin);
            double a = Math.Sin(dLat/2) * Math.Sin(dLat/2) + Math.Cos(radiano(latOrigin)) * Math.Cos(radiano(latDestiny)) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1-a));
            double distance = R * c;
            return int.Parse(Math.Round(distance,0).ToString());
        }

        public double radiano(double degree) {
            return degree * Math.PI / 180;
        }

    }
}