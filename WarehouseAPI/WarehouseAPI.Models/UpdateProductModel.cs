//The Data Model (or DTO/Model layer) focuses on data representation and communication 
//(whether for storage, transfer, or API exposure).

using System;
using System.ComponentModel.DataAnnotations;

namespace WarehouseAPI.Models
{

    public class UpdateProductModel
    {

        [Required]
        public Guid Id { get; set; }

        [Required]
        public int? ProductId { get; set; }

        [Required]
        public string ProductName { get; set; } 

        [Required]
        public int? ProductStoredQuantity { get; set; }

        public int? ProductBookedQuantity { get; set; }

        [Required]
        public int? ProductStorage  { get; set; }
    }
}