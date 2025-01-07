//it’s a good idea to encapsulate the enum within a Value Object in the Domain Model. This approach allows us 
//to apply business logic or validation related to that field more easily.

//In this case, we can create a Value Object that encapsulates the enum. The goal is to provide richer behavior 
//and ensure that the enum is used properly within the domain
using Microsoft.EntityFrameworkCore;

[Keyless]
 public class StatusList
{
    //private readonly StatusListEnum _status;

    // Enum that is encapsulated
    public StatusListEnum Status { get; set; }
    

    public enum StatusListEnum
    {
        Created = 1,
        Updated = 2,
        Booked = 3,
        Delivered = 4,
        Cancelled = 5
    }

    // EF Core will use this parameterless constructor to instantiate the entity
    public StatusList() { }

    public StatusList(StatusListEnum status)
    {
        // Validate or enforce rules here if needed
        if (status == StatusListEnum.Cancelled)
        {
            // Example rule: a cancelled order cannot be shipped
            throw new InvalidOperationException("A cancelled order cannot be delivered");
        }

        Status = status;
    }




}   