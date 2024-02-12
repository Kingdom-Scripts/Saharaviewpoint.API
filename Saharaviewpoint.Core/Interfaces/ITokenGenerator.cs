using Saharaviewpoint.Core.Models.App;
using Saharaviewpoint.Core.Models.Utilities;

namespace Saharaviewpoint.Core.Interfaces;

public interface ITokenGenerator
{
    Task<Result> GenerateJwtToken(User user);

    Task<Result> RefreshJwtToken(string refreshToken);

    Task InvalidateToken(string userReference);
}