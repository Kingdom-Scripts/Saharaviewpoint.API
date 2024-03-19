using Saharaviewpoint.Core.Models.Input.Project;
using Saharaviewpoint.Core.Models.Utilities;

namespace Saharaviewpoint.Core.Interfaces;

public interface IProjectService
{
    #region PROJECTS

    Task<Result> CreateProject(ProjectModel model);

    Task<Result> UpdateProject(int id, ProjectModel model);

    Task<Result> GetProject(int id);

    Task<Result> DeleteProject(int id);

    Task<Result> ListProjects(ProjectSearchModel paging);

    Task<Result> CountProjects();

    Task<Result> ReassignProject(int id, ReassignProjectModel model);

    Task<Result> UpdateProjectStatus(int id, ProjectStatusModel model);

    #endregion PROJECTS

    #region TYPES

    Task<Result> CreateType(TaskModel model);

    Task<Result> DeleteType(int id);

    Task<Result> ListTypes(string? searchTerm);

    #endregion TYPES
}