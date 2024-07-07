using Microsoft.AspNetCore.Mvc;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Models.Input.Client;
using Saharaviewpoint.Models.Utilities;
using Saharaviewpoint.Models.View.Client;

namespace Saharaviewpoint.API.Controllers;

[ApiController]
[Route("api/v1/clients")]
public class ClientController(IClientService clientService) : BaseController
{
    private readonly IClientService _clientService = clientService ?? throw new ArgumentNullException(nameof(clientService));

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult<IEnumerable<ClientView>>))]
    public async Task<IActionResult> ListClients([FromQuery] ClientSearchModel paging)
    {
        var result = await _clientService.ListClients(paging);
        return ProcessResponse(result);
    }
}
