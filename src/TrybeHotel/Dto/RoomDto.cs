namespace TrybeHotel.Dto {
     public class RoomDto {
          public int roomId { get; set; }
          public string? name { get; set; }
          public int capacity { get; set; }
          public string? image { get; set; }
          public HotelDto? hotel { get; set; }
     }
}