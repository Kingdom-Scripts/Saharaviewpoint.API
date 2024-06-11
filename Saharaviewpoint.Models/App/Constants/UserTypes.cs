namespace Saharaviewpoint.Models.App.Constants;

public static class UserTypes
{
    public const string SVP = "SVP Official";
    public const string SVP_ADMIN = "SVP Admin";
    public const string SVP_MANAGER = "SVP Manager";
    public const string BUSINESS_MANAGER = "Business Manager";
    public const string BUSINESS_CLIENT = "Business Client";
    public const string CLIENT = "Client";

    public const string DB_CONSTRAINT =
        $"'{SVP}', '{SVP_ADMIN}', '{SVP_MANAGER}', '{BUSINESS_MANAGER}', '{BUSINESS_CLIENT}', '{CLIENT}'";
}