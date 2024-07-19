// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using Saharaviewpoint.Models.Input.ProjectManager;
using Saharaviewpoint.Models.Input.User;
using Saharaviewpoint.Models.Utilities;

namespace Saharaviewpoint.Core.Interfaces;

public interface IProjectManagerService
{
    Task<Result> ListProjectManagers(ProjectManagerSearchModel request);
    Task<Result> InviteProjectManager(ProjectManagerModel model);
    Task<Result> AcceptInvitation(AcceptInvitationModel model);
    Task<Result> SuspendUser(string userUid);
    Task<Result> ActivateUser(string userUid);
    Task<Result> CheckIfEmailExist(string email);
}