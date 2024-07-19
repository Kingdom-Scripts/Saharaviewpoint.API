// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using Saharaviewpoint.Models.Input;
using Saharaviewpoint.Models.Input.Project;
using Saharaviewpoint.Models.Utilities;

namespace Saharaviewpoint.Core.Interfaces;

public interface IProjectService
{
    #region PROJECTS

    Task<Result> CreateProject(ProjectModel model);

    Task<Result> ApproveProject(int id, string assigneeUid);

    Task<Result> UpdateProject(int id, ProjectModel model);

    Task<Result> GetProject(int id);

    Task<Result> DeleteProject(int id);

    Task<Result> ListProjects(ProjectSearchModel paging);

    Task<Result> CountProjects();

    Task<Result> ReassignProject(int id, ReassignProjectModel model);

    Task<Result> UpdateProjectStatus(int id, ProjectStatusModel model);

    Task<Result> ListProjectLogs(int id, PagingOptionModel request);
    Task<Result> CompleteProject(int id);

    #endregion PROJECTS

    #region TYPES

    Task<Result> CreateType(ProjectTypeModel model);

    Task<Result> DeleteType(int id);

    Task<Result> ListTypes(string? searchTerm);

    #endregion TYPES
}