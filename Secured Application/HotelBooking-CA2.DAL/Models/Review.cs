using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking_CA2.Models;

[Index("RoomId", Name = "IX_Reviews_RoomId")]
[Index("UserId", Name = "IX_Reviews_UserId")]
public partial class Review
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    public int RoomId { get; set; }

    public int Rating { get; set; }

    public string Comment { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    [ForeignKey("RoomId")]
    [InverseProperty("Reviews")]
    public virtual Room Room { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Reviews")]
    public virtual User User { get; set; } = null!;
}
