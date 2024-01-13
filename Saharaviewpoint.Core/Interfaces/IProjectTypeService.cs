using Saharaviewpoint.Core.Models.Utilities;

namespace Saharaviewpoint.Core.Interfaces;

public interface IProjectTypeService
{
    Task<Result> CreateType(string name);
    Task<Result> DeleteType(int id);
    Task<Result> ListTypes();
}
