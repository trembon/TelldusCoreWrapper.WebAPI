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

        [HttpPost("/devices/{id}/{command}")]
        public ActionResult<ResultCode> Get(int id, DeviceMethods command, string parameter = null)
        {
            ResultCode resultCode = telldusCoreService.SendCommand(id, command, parameter);
            return Ok(resultCode);
        }
    }
}