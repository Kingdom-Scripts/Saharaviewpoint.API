using Saharaviewpoint.Models.Input;
using Saharaviewpoint.Models.Input.Project;
using Saharaviewpoint.Models.Utilities;

namespace Saharaviewpoint.Core.Interfaces;

public interface IApprovalService
{
    Task<Result> SendTaskSetupForApproval(int projectId);
    Task<Result> SendTaskSetupApprovalReminder(int projectId, int id);
    Task<Result> GetTaskSetupApproval(int projectId);
    Task<Result> ApproveTaskSetup(int projectId, int id, TaskApprovalModel model);
    Task<Result> ListApprovalRequests(PagingOptionModel model);
}