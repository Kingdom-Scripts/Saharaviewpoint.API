// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

namespace Saharaviewpoint.Core.Contants.CacheKeys;
internal static partial class CacheKeys
{
    internal static string TaskApprovalRequest() => "TaskApprovalRequest-CacheKeys";
    internal static string TaskSetupApproval(int projectId) => $"Approval-TaskSetupApproval-{projectId}";
    internal static string ListApprovalRequests(string generatedKey) => $"Approval-ListApprovalRequests-{generatedKey}";
}
