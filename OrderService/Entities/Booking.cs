﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.Enums;

namespace OrderService.Entities
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        public BookingStatus BookingStatus { get; set; } = BookingStatus.Pending;
        public PaymentMethodType PaymentMethod { get; set; }
        public int ServiceId { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public int TaskerId { get; set; }
        public DateTime ScheduleTime { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public ICollection<BookingDuration> BookingDurations { get; set; } = new List<BookingDuration>();
        public ICollection<BookingPremium> BookingPremiums { get; set; } = new List<BookingPremium>();
    }
}
