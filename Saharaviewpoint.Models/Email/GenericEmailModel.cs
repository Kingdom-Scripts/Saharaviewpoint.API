// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using Microsoft.AspNetCore.Http;

namespace Saharaviewpoint.Models.Email;
public class GenericEmailModel
{
    public List<EmailAddress> To { get; set; } = [];
    public List<EmailAddress> Cc { get; set; } = [];
    public List<EmailAddress> Bcc { get; set; } = [];
    public required string Subject { get; set; }

    public required string Salutation { get; set; }
    public required string PrimaryMessage { get; set; }
    public EmailActionButton ActionButton { get; set; }
    public string SecondaryMessage { get; set; }
    public string ClosingRemark { get; set; }
    public bool ShowAlternateUrl { get; set; } = false;
    public List<IFormFile> Attachments { get; set; } = [];
}

public class EmailActionButton
{
    public required string Text { get; set; }
    public required string Url { get; set; }
}