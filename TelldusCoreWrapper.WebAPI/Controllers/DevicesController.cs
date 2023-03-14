using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TelldusCoreWrapper.Entities;

namespace TelldusCoreWrapper.WebAPI.Controllers
{
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly ITelldusCoreService telldusCoreService;

        public DevicesController(ITelldusCoreService telldusCoreService)
        {
            this.telldusCoreService = telldusCoreService;
        }

        [HttpGet("/devices/")]
        public async Task<ActionResult<IEnumerable<Device>>> GetAll()
        {
            return await Task.Factory.StartNew(() =>
            {
                IEnumerable<Device> devices = telldusCoreService.GetDevices();
                return Ok(devices);
            });
        }
        
        [HttpGet("/devices/{id}")]
        public async Task<ActionResult<Device>> Get(int id)
        {
            return await Task.Factory.StartNew<ActionResult<Device>>(() =>
            {
                Device device = telldusCoreService.GetDevice(id);
                if (device == null)
                    return NotFound();

                return Ok(device);
            });
        }

        [HttpDelete("/devices/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return await Task.Factory.StartNew(() =>
            {
                bool result = telldusCoreService.RemoveDevice(id);
                return Ok(result);
            });
        }
    }
}
