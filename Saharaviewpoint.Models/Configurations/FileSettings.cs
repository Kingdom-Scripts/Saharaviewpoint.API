// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

namespace Saharaviewpoint.Models.Configurations;

public class FileSettings
{
    public string FilePath { get; set; }
    public string RequestPath { get; set; }
    public int MaxSizeLength { get; set; }
    public List<string> PermittedFileTypes { get; set; }
}