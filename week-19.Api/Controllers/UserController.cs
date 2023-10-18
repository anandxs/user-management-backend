using Microsoft.AspNetCore.Mvc;
using week_19.Api.Models;
using week_19.Api.Models.Dto;
using week_19.Api.Services;

namespace week_19.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController: ControllerBase
    {
        private readonly UsersService _db;

        public UserController(UsersService db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string query)
        {
            var userList = await _db.SearchAsync(query);

            return Ok(userList);
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> GetUser(string email)
        {
            var user = await _db.GetAsync(email);

            if (user is null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> Register([FromBody]RegisterDto newUser)
        {
            if (newUser is null || newUser.Email == String.Empty)
                return BadRequest();

            User model = new()
            {
                Email = newUser.Email,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Password = newUser.Password,
                Role = "user",
            };

            await _db.CreateAsync(model);

            return Ok(model);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, EditDto updatedUser)
        {
            var user = await _db.GetAsync(id);

            if (user is null)
                return NotFound();

            User model = new()
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role,
                FirstName = updatedUser.FirstName,
                LastName = updatedUser.LastName,
                Password = updatedUser.Password,
                ImageUrl = updatedUser.ImageUrl
            };

            await _db.UpdateAsync(id, model);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _db.GetAsync(id);

            if (user is null)
                return NotFound();

            bool status = await _db.DeleteAsync(id);

            if (status)
                return NoContent();

            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
