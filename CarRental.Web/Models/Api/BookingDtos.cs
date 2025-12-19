using System;
using System.ComponentModel.DataAnnotations;

namespace CarRental.Web.Models.Api
{
    public class BookingDto
    {
        public long Id { get; set; }
        public long VehicleId { get; set; }
        public string VehicleName { get; set; } = string.Empty;
        public long ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PickUpLocation { get; set; } = string.Empty;
        public string DropOffLocation { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsPaid { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateBookingDto
    {
        public long VehicleId { get; set; }
        public long ClientId { get; set; }

        [Required(ErrorMessage = "La date de début est requise")]
        [Display(Name = "Date de début")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "La date de fin est requise")]
        [Display(Name = "Date de fin")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Le lieu de prise en charge est requis")]
        [Display(Name = "Lieu de prise en charge")]
        [StringLength(500, ErrorMessage = "Le lieu ne peut pas dépasser 500 caractères")]
        public string PickUpLocation { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le lieu de restitution est requis")]
        [Display(Name = "Lieu de restitution")]
        [StringLength(500, ErrorMessage = "Le lieu ne peut pas dépasser 500 caractères")]
        public string DropOffLocation { get; set; } = string.Empty;

        [Display(Name = "Notes supplémentaires")]
        [StringLength(2000, ErrorMessage = "Les notes ne peuvent pas dépasser 2000 caractères")]
        public string? Notes { get; set; }
    }

    public class ReturnVehicleDto
    {
        public long BookingId { get; set; }
        public DateTime ReturnDate { get; set; }
        public int FinalMileage { get; set; }
        public bool IsDamaged { get; set; }
        public string? DamageDescription { get; set; }
    }
}
