using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TelldusCoreWrapper.Entities;

namespace TelldusCoreWrapper.WebAPI.Controllers
{
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private ITelldusCoreService telldusCoreService;

        public DevicesController(ITelldusCoreService telldusCoreService)
        {
            this.telldusCoreService = telldusCoreService;
        }
        
        [HttpGet("/devices/")]
        public ActionResult<IEnumerable<Device>> Get()
        {
            IEnumerable<Device> devices = telldusCoreService.GetDevices();
            return Ok(devices);
        }
        
        [HttpGet("/devices/{id}")]
        public ActionResult<Device> Get(int id)
        {
            Device device = telldusCoreService.GetDevice(id);
            if (device == null)
                return NotFound();

            return Ok(device);
        }
        [HttpPost("/devices/{id}/{command}")]
        public ActionResult<ResultCode> Get(int id, string command, [FromBody] string parameter = null)
        {
            //ResultCode resultCode = telldusCoreService.SendCommand(id, command, parameter);
            //return Ok(resultCode);
            return Ok(ResultCode.Success);
        }

        //// POST api/values
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        [HttpDelete("/devices/{id}")]
        public ActionResult<bool> Delete(int id)
        {
            bool result = telldusCoreService.RemoveDevice(id);
            return Ok(result);
        }
    }
}
