using System.ComponentModel;

namespace Saharaviewpoint.Core.Models.App.Enums
{
    public enum UserTypes
    {
        [Description("Business")]
        Business = 1,
        [Description("Client")]
        Client = 2,
        [Description("Manager")]
        Manager = 3
    }
}
