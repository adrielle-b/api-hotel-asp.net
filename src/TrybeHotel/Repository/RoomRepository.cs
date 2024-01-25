using TrybeHotel.Models;
using TrybeHotel.Dto;
using Microsoft.EntityFrameworkCore;

namespace TrybeHotel.Repository
{
    public class RoomRepository : IRoomRepository
    {
        protected readonly ITrybeHotelContext _context;
        public RoomRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        // 7. Refatore o endpoint GET /room
        public IEnumerable<RoomDto> GetRooms(int HotelId)
        {
            return _context.Rooms
                .Select(room => new RoomDto
                {
                    roomId = room.RoomId,
                    name = room.Name,
                    capacity = room.Capacity,
                    image = room.Image,
                    hotel = new HotelDto
                            {
                                hotelId = HotelId,
                                name = room.Hotel!.Name,
                                address = room.Hotel.Address,
                                cityId = room.Hotel.CityId,
                                cityName = room.Hotel.City!.Name,
                                state = room.Hotel.City!.State,
                            }
                }).ToList();
        }

        // 8. Refatore o endpoint POST /room
        public RoomDto AddRoom(Room room) 
        {
             _context.Rooms.Add(room);
            _context.SaveChanges();

            var roomAdded = _context.Rooms
                .Include(r => r.Hotel)
                .ThenInclude(h => h!.City)
                .FirstOrDefault(r => r.RoomId == room.RoomId);
            
            return new RoomDto 
            {
                roomId = roomAdded!.RoomId,
                name = roomAdded.Name,
                capacity = roomAdded.Capacity,
                image = roomAdded.Image,
                hotel = new HotelDto
                        {
                            hotelId = roomAdded.Hotel!.HotelId,
                            name = roomAdded.Hotel.Name,
                            address = roomAdded.Hotel.Address,
                            cityId = roomAdded.Hotel.CityId,
                            cityName = roomAdded.Hotel.City!.Name,
                            state = roomAdded.Hotel.City!.State,
                        }
            };
        }

        public void DeleteRoom(int RoomId) 
        {
          var roomRemoving = (from room in _context.Rooms
                                where room.RoomId == RoomId
                                select room).First();
            _context.Rooms.Remove(roomRemoving);
            _context.SaveChanges();
        }
    }
}