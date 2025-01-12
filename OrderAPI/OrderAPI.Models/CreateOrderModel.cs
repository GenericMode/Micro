//The Data Model (or DTO/Model layer) focuses on data representation and communication 
//(whether for storage, transfer, or API exposure).

using System;
using System.ComponentModel.DataAnnotations;

namespace OrderAPI.Models
{
    public enum StatusListModel
    {
        Created = 1,
        Updated = 2,
        Booked = 3,
        Delivered = 4,
        Cancelled = 5
    }

    public class CreateOrderModel
    {
        [Required]
        public string CompanyName { get; set; }

        [Required]
        public int? ProductId { get; set; }

        [Required]
        public string ProductName { get; set; } //later save it as a list??

        [Required]
        public int? ProductQuantity { get; set; }

        public DateTime? OrderDate { get; set; }

        public StatusListModel StatusId { get; set; }
    }
}