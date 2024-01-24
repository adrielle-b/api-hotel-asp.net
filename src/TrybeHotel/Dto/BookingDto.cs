using TrybeHotel.Models;

namespace TrybeHotel.Dto
{
    public class BookingDtoInsert
    {
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int GuestQuant { get; set; }
        public int RoomId { get; set; } 
    }

    public class BookingResponse
    {
        public int BookingId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int GuestQuant { get; set; }
        public RoomDto? Room { get; set; }

    }
}