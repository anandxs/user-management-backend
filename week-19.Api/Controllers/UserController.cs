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

        [HttpPost("img")]
        public async Task<IActionResult> UploadImage([FromBody] ImageDto imageDto)
        {
            var user = await _db.GetAsync(imageDto.Id);

            try
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imageDto.Img.FileName);

                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    imageDto.Img.CopyTo(stream);
                }

                return Ok(path);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            var userList = await _db.SearchAsync(query);

            return Ok(userList);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userList = await _db.GetAsync();

            return Ok(userList);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _db.GetAsync(id);

            if (user is null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> Register([FromBody]RegisterDto newUser)
        {
            if (newUser is null || newUser.Email == String.Empty)
                return BadRequest();

            var existingUser = await _db.CheckEmailAsync(newUser.Email);

            if (existingUser is not null)
            {
                ModelState.AddModelError("emailTaken", "Email is already taken");
                return BadRequest(ModelState);
            }

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
        public async Task<IActionResult> Update(string id, [FromBody]EditDto updatedUser)
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
                Password = user.Password,
            };

            if (updatedUser.Password is not "")
                model.Password = updatedUser.Password;

            await _db.UpdateAsync(id, model);

            return Ok(new {imageUrl = 'x', firstName = model.FirstName, lastName = model.LastName});
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
