namespace PraceticeAssesment.Entity.Models;

public class BaseEntity
{
    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }
}
