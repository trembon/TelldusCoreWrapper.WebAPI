using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TelldusCoreWrapper.Enums;

namespace TelldusCoreWrapper.WebAPI.Controllers
{
    [ApiController]
    public class CommandController : ControllerBase
    {
        private readonly ITelldusCoreService telldusCoreService;

        public CommandController(ITelldusCoreService telldusCoreService)
        {
            this.telldusCoreService = telldusCoreService;
        }

        [HttpPost("/devices/{id}/send/{command}")]
        public async Task<ActionResult<ResultCode>> SendCommand(int id, DeviceMethods command, string parameter = null)
        {
            ResultCode result = await Task.Factory.StartNew(() => telldusCoreService.SendCommand(id, command, parameter));
            return Ok(result);
        }

        [HttpGet("/devices/{id}/lastcommand")]
        public async Task<ActionResult<ResultCode>> GetLastCommand(int id)
        {
            return await Task.Factory.StartNew(() =>
            {
                DeviceMethods lastCommand = telldusCoreService.GetLastCommand(id);
                return Ok(lastCommand);
            });
        }
    }
}