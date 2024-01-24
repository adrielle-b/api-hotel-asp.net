using TrybeHotel.Models;
using TrybeHotel.Dto;

namespace TrybeHotel.Repository
{
    public class HotelRepository : IHotelRepository
    {
        protected readonly ITrybeHotelContext _context;
        public HotelRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        //  5. Refatore o endpoint GET /hotel
        public IEnumerable<HotelDto> GetHotels()
        {
            var hotels = _context.Hotels
                .Select(hotel => new HotelDto
                {
                    hotelId = hotel.HotelId,
                    name = hotel.Name,
                    address = hotel.Address,
                    cityId = hotel.CityId,
                    cityName = (from city in _context.Cities
                            where city.CityId == hotel.CityId
                            select city.Name).FirstOrDefault(),
                });
            return hotels.ToList();
        }

        // 6. Refatore o endpoint POST /hotel
        public HotelDto AddHotel(Hotel hotel)
        {
            _context.Hotels.Add(hotel);
            _context.SaveChanges();

            return new HotelDto
            {
                hotelId = hotel.HotelId,
                name = hotel.Name,
                address = hotel.Address,
                cityId = hotel.CityId,
                cityName = (from city in _context.Cities
                            where city.CityId == hotel.CityId
                            select city.Name).FirstOrDefault(),
            };
        }
    }
}