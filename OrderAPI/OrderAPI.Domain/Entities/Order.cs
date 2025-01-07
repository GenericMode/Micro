//By having a separate Domain Model, you can keep your business rules isolated from the data storage or transportation layer. 
//For example, your domain objects might have complex behavior, validation, and interactions, but the data model might be 
//simple (just representing data as JSON or as a database table). This separation allows you to modify one layer 
//(e.g., how data is stored or transferred) without directly affecting the other layer (e.g., how business logic is structured).

using System;

namespace OrderAPI.Domain.Entities
{
    public class Order
     {
        public Guid Id { get; set; }
        public string CompanyName { get; set; }
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        public int? ProductQuantity { get; set; }
        public DateTime? OrderDate { get; set; }
        public StatusList.StatusListEnum StatusId { get; set; }

        // Parameterless constructor
        public Order() { }
         public Order(Guid Id, string CompanyName, int? ProductId, string ProductName, int? ProductQuantity, DateTime? OrderDate, StatusList StatusId)
        {
            Id = Id;
            CompanyName = CompanyName;
            ProductId  = ProductId;
            ProductName  = ProductName;
            ProductQuantity = ProductQuantity;
            OrderDate = OrderDate;
            StatusId = StatusId;
        }
     
    // Methods to transition states can be added here
    public void BookOrder()
    {
        if (StatusId != StatusList.StatusListEnum.Booked)
        {
            throw new InvalidOperationException("Only booked orders can be delivered.");
        }

        StatusId = StatusList.StatusListEnum.Booked;
    }

    }

}