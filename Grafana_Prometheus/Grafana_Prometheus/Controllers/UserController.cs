using Grafana_Prometheus.Interfaces;
using Grafana_Prometheus.Models;
using Microsoft.AspNetCore.Mvc;

namespace Grafana_Prometheus.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly IUsuarioService _service;

        public UserController(IUsuarioService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult> Add([FromBody] User user)
        {
            if ((await _service.GetAsync(user.Name)).Any())
                throw new Exception("User already exist");

            await _service.AddAsync(user);
            return Created("", null);
        }

        [HttpDelete("{name}")]
        public async Task<ActionResult> Delete([FromRoute] string name)
        {
            if (!(await _service.GetAsync(name)).Any())
                return NotFound();

            await _service.RemoveAsync(name);
            return Ok();
        }

        [HttpGet("{name}")]
        public async Task<ActionResult> GetByName([FromRoute] string name)
        {
            var user = (await _service.GetAsync(name)).FirstOrDefault();
            return user is null ? NoContent() : Ok(user);
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var users = await _service.GetAsync(null);
            return !users.Any() ? NoContent() : Ok(users);
        }
    }
}
