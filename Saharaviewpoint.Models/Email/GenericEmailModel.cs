// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

namespace Saharaviewpoint.Models.Email;
public class GenericEmailModel
{
    public required string To { get; set; }
    public string? Cc { get; set; }
    public string? Bcc { get; set; }
    public required string Subject { get; set; }

    public required string Salutation { get; set; }
    public required string PrimaryMessage { get; set; }
    public EmailActionButton? ActionButton { get; set; }
    public string? SecondaryMessage { get; set; }
    public string? ClosingRemark { get; set; }
}

public class EmailActionButton
{
    public required string Text { get; set; }
    public required string Url { get; set; }
}