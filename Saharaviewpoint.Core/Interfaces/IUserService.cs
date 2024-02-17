using Saharaviewpoint.Core.Models.Utilities;

namespace Saharaviewpoint.Core.Interfaces;

public interface IUserService
{
    Task<Result> UserProfile();

    

    Task<Result> AddManger();
}