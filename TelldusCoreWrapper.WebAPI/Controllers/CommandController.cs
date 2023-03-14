using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TelldusCoreWrapper.Enums;
using TelldusCoreWrapper.WebAPI.Services;

namespace TelldusCoreWrapper.WebAPI.Controllers
{
    [ApiController]
    public class CommandController : ControllerBase
    {
        private readonly ITelldusCoreService telldusCoreService;
        private readonly ITelldusCommandService telldusCommandService;

        public CommandController(ITelldusCoreService telldusCoreService, ITelldusCommandService telldusCommandService)
        {
            this.telldusCoreService = telldusCoreService;
            this.telldusCommandService = telldusCommandService;
        }

        [HttpPost("/devices/{id}/send/{command}")]
        public async Task<ActionResult> SendCommand(int id, DeviceMethods command, string parameter = null)
        {
            await telldusCommandService.SendCommand(id, command, parameter);
            return Ok();
        }

        [HttpGet("/devices/{id}/lastcommand")]
        public async Task<ActionResult<DeviceMethods>> GetLastCommand(int id)
        {
            return await Task.Factory.StartNew(() =>
            {
                DeviceMethods lastCommand = telldusCoreService.GetLastCommand(id);
                return Ok(lastCommand);
            });
        }
    }
}