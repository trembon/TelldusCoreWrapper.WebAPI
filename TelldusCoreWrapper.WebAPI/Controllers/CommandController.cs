using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TelldusCoreWrapper.Enums;

namespace TelldusCoreWrapper.WebAPI.Controllers
{
    [ApiController]
    public class CommandController : ControllerBase
    {
        private ITelldusCoreService telldusCoreService;

        public CommandController(ITelldusCoreService telldusCoreService)
        {
            this.telldusCoreService = telldusCoreService;
        }

        [HttpPost("/devices/{id}/send/{command}")]
        public async Task<ActionResult<ResultCode>> SendCommand(int id, DeviceMethods command, string parameter = null, int retry = 1)
        {
            ResultCode latestResultCode = ResultCode.UnknownError;

            for(int i = 0; i < retry; i++)
                latestResultCode = await Task.Factory.StartNew(() => telldusCoreService.SendCommand(id, command, parameter));

            return Ok(latestResultCode);
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