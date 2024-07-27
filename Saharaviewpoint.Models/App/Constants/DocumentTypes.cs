// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

namespace Saharaviewpoint.Models.App.Constants;

public class DocumentTypes
{
    public const string IMAGE = "Image";
    public const string PDF = "PDF";
    public const string WORD_DOCUMENT = "Word";
    public const string EXCEL_DOCUMENT = "EXCEL";
    public const string UNKNWON = "Unknown";

    public const string DB_CONSTRAINT =
        $"'{IMAGE}', '{PDF}', '{WORD_DOCUMENT}', '{EXCEL_DOCUMENT}', '{UNKNWON}'";
}