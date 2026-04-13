using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking_CA2.Models;

public partial class Room
{
    [Key]
    public int Id { get; set; }

    public string RoomNumber { get; set; } = null!;

    public string RoomType { get; set; } = null!;

    public string Description { get; set; } = null!;

    [Column(TypeName = "decimal(10, 2)")]
    public decimal PricePerNight { get; set; }

    public int Capacity { get; set; }

    public bool IsAvailable { get; set; }

    public string ImageUrl { get; set; } = null!;

    [InverseProperty("Room")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    [InverseProperty("Room")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
