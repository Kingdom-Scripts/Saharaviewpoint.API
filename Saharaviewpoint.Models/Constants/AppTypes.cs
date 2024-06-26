namespace Saharaviewpoint.Models.Constants;

public class AppTypes
{

    public static readonly AppTypes Client = new("Client");
    public static readonly AppTypes Admin = new("Admin");

    public string Value { get; }

    private AppTypes(string value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Value;
    }
}