using TrybeHotel.Models;
using TrybeHotel.Dto;
using Microsoft.EntityFrameworkCore;

namespace TrybeHotel.Repository
{
    public class BookingRepository : IBookingRepository
    {
        protected readonly ITrybeHotelContext _context;
        public BookingRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        // 9. Refatore o endpoint POST /booking
        public BookingResponse Add(BookingDtoInsert booking, string email)
        {
            var room = GetRoomById(booking.RoomId);
            if (booking.GuestQuant > room.Capacity) return null!;

           var user = _context.Users.FirstOrDefault(u => u.Email == email);
            var newBooking = new Booking
            {
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                GuestQuant = booking.GuestQuant,
                UserId = user!.UserId,
                RoomId = room.RoomId
            };

            _context.Bookings.Add(newBooking);
            _context.SaveChanges();

            var bookingAdded = _context.Bookings
                .Include(b => b.Room)
                .ThenInclude(r => r!.Hotel)
                .ThenInclude(h => h!.City)
                .FirstOrDefault(b => b.BookingId == newBooking.BookingId);
            
            return new BookingResponse
            {
                BookingId = bookingAdded!.BookingId,
                CheckIn = bookingAdded.CheckIn,
                CheckOut = bookingAdded.CheckOut,
                GuestQuant = bookingAdded.GuestQuant,
                Room = new RoomDto
                {
                    roomId = bookingAdded.Room!.RoomId,
                    name = bookingAdded.Room.Name,
                    capacity = bookingAdded.Room.Capacity,
                    image = bookingAdded.Room.Image,
                    hotel = new HotelDto
                    {
                        hotelId = bookingAdded.Room.Hotel!.HotelId,
                        name = bookingAdded.Room.Hotel.Name,
                        address = bookingAdded.Room.Hotel.Address,
                        cityId = bookingAdded.Room.Hotel.CityId,
                        cityName = bookingAdded.Room.Hotel.City!.Name,
                        state = bookingAdded.Room.Hotel.City!.State,
                    },
                }
            };
        }

        // 10. Refatore o endpoint GET /booking
        public BookingResponse GetBooking(int bookingId, string email)
        {
            var bookingValid =  _context.Bookings.Include(u => u.User).FirstOrDefault(b => b.BookingId == bookingId);
            if (bookingValid!.User!.Email != email) return null!;

            var booking = _context.Bookings
                .Include(b => b.Room)
                .ThenInclude(r => r!.Hotel)
                .ThenInclude(h => h!.City)
                .FirstOrDefault(b => b.BookingId == bookingId);

            return new BookingResponse
            {
                BookingId = booking!.BookingId,
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                GuestQuant = booking.GuestQuant,
                Room = new RoomDto
                {
                    roomId = booking.Room!.RoomId,
                    name = booking.Room.Name,
                    capacity = booking.Room.Capacity,
                    image = booking.Room.Image,
                    hotel = new HotelDto
                    {
                        hotelId = booking.Room.Hotel!.HotelId,
                        name = booking.Room.Hotel.Name,
                        address = booking.Room.Hotel.Address,
                        cityId = booking.Room.Hotel.CityId,
                        cityName = booking.Room.Hotel.City!.Name,
                        state = booking.Room.Hotel.City!.State,
                    },
                }
            };
        }

        public Room GetRoomById(int RoomId)
        {
            var room = _context.Rooms.FirstOrDefault(r => r.RoomId == RoomId);
            return room!;
            
        }

    }

}