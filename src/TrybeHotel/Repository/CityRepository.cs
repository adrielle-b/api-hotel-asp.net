using TrybeHotel.Models;
using TrybeHotel.Dto;

namespace TrybeHotel.Repository
{
    public class CityRepository : ICityRepository
    {
        protected readonly ITrybeHotelContext _context;
        public CityRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        public IEnumerable<CityDto> GetCities()
         {
            var cities = _context.Cities
                .Select(cities => new CityDto
                {
                    cityId = cities.CityId,
                    name = cities.Name,
                    state = cities.State,
                });
            return cities.ToList();
        }

        public CityDto AddCity(City city)
        {
            _context.Cities.Add(city);
            _context.SaveChanges();
            return new CityDto
            {
                cityId = city.CityId,
                name = city.Name,
                state = city.State,
            };
        }

        public CityDto UpdateCity(City city)
        {
            _context.Cities.Update(city);
            _context.SaveChanges();
            return new CityDto
            {
                cityId = city.CityId,
                name = city.Name,
                state = city.State,
            };
        }

    }
}