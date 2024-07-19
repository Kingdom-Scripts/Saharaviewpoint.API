// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

namespace Saharaviewpoint.Models.App.Constants;

public class TaskStatusEnum
{
    public const string TODO = "TO DO";
    public const string IN_PROGRESS = "IN PROGRESS";
    public const string COMPLETED = "COMPLETED";

    public static bool IsGoingBack(string previousState, string status)
    {
        return previousState == COMPLETED && status == IN_PROGRESS
         || previousState == IN_PROGRESS && status == TODO;
    }

    public static bool IsValidTransition(string currentStatus, string status)
    {
        return currentStatus == TODO && status == IN_PROGRESS
         || currentStatus == IN_PROGRESS && status == COMPLETED
         || currentStatus == IN_PROGRESS || currentStatus == COMPLETED;
    }
}