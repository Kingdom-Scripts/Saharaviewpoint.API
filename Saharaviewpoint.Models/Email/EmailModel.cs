// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using Microsoft.AspNetCore.Http;

namespace Saharaviewpoint.Models.Email;

public class EmailModel
{
    public EmailAddress From { get; set; }
    public List<EmailAddress> To { get; set; } = [];
    public List<EmailAddress> Cc { get; set; } = [];
    public List<EmailAddress> Bcc { get; set; } = [];
    public string Subject { get; set; }
    public string HtmlBody { get; set; }
    public object MetaData { get; set; }
    public List<IFormFile> Attachments { get; set; } = [];
}