// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

namespace Saharaviewpoint.Models.App;

public class Login : BaseAppModel
{
    public int UserId { get; set; }
    public required string HashedToken { get; set; }
    public required string Domain { get; set; }
    public required DateTime ExpiresAt { get; set; }

    public User? User { get; set; }
}
