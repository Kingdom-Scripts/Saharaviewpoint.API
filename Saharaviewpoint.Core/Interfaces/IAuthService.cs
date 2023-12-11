using Saharaviewpoint.Core.Models.Input;
using Saharaviewpoint.Core.Models.Utilities;

namespace Saharaviewpoint.Core.Interfaces;

public interface IAuthService
{
    Result CreateUser(RegisterModel model);
}