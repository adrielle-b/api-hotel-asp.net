namespace TrybeHotel.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Booking {
    [Key]
    public int BookingId { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int GuestQuant { get; set; }
    [ForeignKey("UserId")]
    public int UserId { get; set; }
    [ForeignKey("RoomId")]
    public int RoomId { get; set; }
    public User? User { get; set; }
    public Room? Room { get; set; }
}