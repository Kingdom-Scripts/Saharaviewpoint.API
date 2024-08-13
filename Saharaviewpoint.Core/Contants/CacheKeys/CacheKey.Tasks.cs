// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

namespace Saharaviewpoint.Core.Contants.CacheKeys;
internal static partial class CacheKeys
{
    // Tasks
    internal static string TaskDetails(int id) => $"task-details-{id}";
    internal static string ListAttachments(int id) => $"task-attachments-{id}";
    internal static string BoardTasks(int projectId) => $"board-tasks-{projectId}";
    internal static string BoardTasksValidation(int projectId) => $"board-tasks-{projectId}-validation";
    internal static string TaskLogsCacheKeys(int id) => $"TaskService-ListLogs-{id}-CacheKeys";
    internal static string CommentListCacheKeys(int id) => $"TaskService-Comment-{id}-CacheKeys";
    internal static string TaskListCacheKeys() => "TaskService-ListTasks-CacheKeys";
}
