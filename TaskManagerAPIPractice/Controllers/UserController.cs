using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagerAPIPractice.Application.Services;
using TaskManagerAPIPractice.Contracts;
using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // POST: api/users/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserRequest userRequest)
        {
            //try
            //{
            await _userService.Register(userRequest.UserName, userRequest.Email, userRequest.Password);
            return Ok("User registered successfully");
            // }
            //catch (Exception ex)
            //{
            //return BadRequest(new { message = ex.Message });
            // }
        }

        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] LoginUserRequest userRequest)
        //{
        //    var token = await _userService.Login(userRequest.Email, userRequest.Password);

        //    if (token == null)
        //    {
        //        return Unauthorized(); // Якщо логін не вдався
        //    }

        //    // Додаємо токен у кукі
        //    Response.Cookies.Append("tasty-cookies", token, new CookieOptions
        //    {
        //        HttpOnly = true,  // Захист від XSS
        //        Secure = true,    // Кукі працюватиме лише по HTTPS (рекомендується)
        //        SameSite = SameSiteMode.Strict // Захист від CSRF
        //    });

        //    return Ok(new { Token = token });
        //}
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest userRequest)
        {
            var loginResult = await _userService.Login(userRequest.Email, userRequest.Password);

            if (loginResult == null)
            {
                return Unauthorized();
            }

            Response.Cookies.Append("tasty-cookies", loginResult.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            return Ok(new
            {
                Token = loginResult.Token,
                UserId = loginResult.UserId
            });
        }


        // Отримати всіх користувачів
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<UserResponse>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users.Select(MapToUserResponse).ToList());
        }

        // Отримати користувача по ID
        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<ActionResult<UserResponse>> GetUserById(Guid id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null) return NotFound("User not found.");
            return Ok(MapToUserResponse(user));
        }

        // Оновити користувача
        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserRequest request)
        {
            var existingUser = await _userService.GetUserById(id);
            if (existingUser == null) return NotFound("User not found.");

            // Оновлення даних користувача
            if (!string.IsNullOrWhiteSpace(request.FullName)) existingUser.FullName = request.FullName;
            if (!string.IsNullOrWhiteSpace(request.Email)) existingUser.Email = request.Email;
            if (request.TeamId.HasValue) existingUser.TeamId = request.TeamId;

            if (request.Tags != null)
            {
                existingUser.Tags = request.Tags.Select(tagId => new TagEntity { Id = tagId }).ToList();
            }

            await _userService.UpdateUser(existingUser);
            return Ok(MapToUserResponse(existingUser));
        }

        // Видалити користувача
        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var result = await _userService.DeleteUser(id);
            if (!result) return NotFound("User not found.");
            return Ok("User deleted successfully.");
        }

        private static UserResponse MapToUserResponse(UserEntity user)
        {
            return new UserResponse(
                user.Id,
                user.FullName,
                user.Email,
                user.CreatedAt,
                user.Team != null ? new TeamResp(user.Team.Id, user.Team.Name) : null,
                user.Tags.Select(tag => new TagResp(tag.Id, tag.Name)).ToList(),
                user.CreatedTasks.Count
            );
        }
    }
}

