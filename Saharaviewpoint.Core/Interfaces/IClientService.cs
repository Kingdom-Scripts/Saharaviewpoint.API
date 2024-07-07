using Saharaviewpoint.Models.Input.Client;
using Saharaviewpoint.Models.Utilities;

namespace Saharaviewpoint.Core.Interfaces;

public interface IClientService
{
    Task<Result> ListClients(ClientSearchModel request);
}
