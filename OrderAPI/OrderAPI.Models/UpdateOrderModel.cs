using System;
using System.ComponentModel.DataAnnotations;

namespace OrderAPI.Models
{
   
    public class UpdateOrderModel
    {
        [Required]
        public Guid Id { get; set; }
        
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