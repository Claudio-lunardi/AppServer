using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace AppServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StateController : ControllerBase
    {
        private static int _contador = 0;

        [HttpGet("contador")]
        public IActionResult get()
        {
            _contador++;
            return Ok(new { contador = _contador });
        }
    }
}
