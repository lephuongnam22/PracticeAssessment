using AutoMapper;
using PraceticeAssesment.Entity.Models;
using PracticeAssessment.Core.Models;

namespace PracticeAssessment.Mappers;

public class ResponseProfiles : Profile
{
    public ResponseProfiles()
    {
        CreateMap<TaskEntity, TaskModel>();
        CreateMap<TaskModel, TaskEntity>();

        CreateMap<TaskEntity, TaskResponse>();

        CreateMap<TaskCommentEntity, CommentModel>();
        CreateMap<CommentModel, TaskCommentEntity>();

        CreateMap<TaskHistory, TaskHistoryModel>();

        CreateMap<NotificationEntity, NotificationModel>();
    }
}
