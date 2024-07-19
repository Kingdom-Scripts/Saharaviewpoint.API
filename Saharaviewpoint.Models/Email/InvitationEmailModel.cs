// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

namespace Saharaviewpoint.Models.Email;

public class InvitationEmailModel
{
    public required string Token { get; set; }
    public required string UserType { get; set; }
    public required string RecipientName { get; set; }
    public required string RecipientEmail { get; set; }
    public required string SenderName { get; set; }
}