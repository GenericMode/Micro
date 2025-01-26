//By having a separate Domain Model, you can keep your business rules isolated from the data storage or transportation layer. 
//For example, your domain objects might have complex behavior, validation, and interactions, but the data model might be 
//simple (just representing data as JSON or as a database table). This separation allows you to modify one layer 
//(e.g., how data is stored or transferred) without directly affecting the other layer (e.g., how business logic is structured).

using System;

namespace WarehouseAPI.Domain.Entities
{
    public class Product
     {
        public Guid Id { get; set; }
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        public int? ProductStoredQuantity { get; set; }
        public int? ProductBookedQuantity { get; set; }
        public int? ProductStorage { get; set; }

        // Parameterless constructor
        public Product() { }
        public Product(Guid id, int? productId, string productName, int? productStoredQuantity, int? productBookedQuantity, int? productStorage)
        {
            Id = id;
            ProductId  = productId;
            ProductName  = productName;
            ProductStoredQuantity = productStoredQuantity;
            ProductBookedQuantity = productBookedQuantity;
            ProductStorage = productStorage;
        }
     
    // Methods to transition states can be added here
    public void BookFull()
    {
        if (ProductBookedQuantity > ProductStoredQuantity)
        {
            throw new InvalidOperationException("Total booked quantity exceeds stored quantity of product.");
        }

        ProductBookedQuantity = ProductStoredQuantity;
    }

    }

}