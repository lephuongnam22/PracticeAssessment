using Microsoft.AspNetCore.Identity;

namespace PraceticeAssesment.Entity.Models;

public class UserEntity : IdentityUser<int>
{
    public virtual ICollection<RelUserTaskEntity> RelUserTaskEntities { get; set; } = [];

    public virtual ICollection<TaskCommentEntity> TaskCommentEntities { get; set; } = [];
}
