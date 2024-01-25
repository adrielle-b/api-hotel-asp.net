using TrybeHotel.Models;

namespace TrybeHotel.Dto {
     public class HotelDto {
        public int hotelId { get; set; }
        public string? name { get; set; }
        public string? address { get; set; }
        public int cityId { get; set; }
        public string? cityName { get; set; }
        public string? state { get; set; }
    }
}