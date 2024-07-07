using Saharaviewpoint.Models.Input.Client;
using Saharaviewpoint.Models.Utilities;

namespace Saharaviewpoint.Core.Interfaces;

public interface IClientService
{
    Task<Result> ListClients(ClientSearchModel request);
    Task<Result> DeactivateClient(string uid);
    Task<Result> ActivateClient(string uid);
}
