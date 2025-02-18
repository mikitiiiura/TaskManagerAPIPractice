using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagerAPIPractice.Application.Services;
using TaskManagerAPIPractice.Contracts;
using TaskManagerAPIPractice.Contracts.Request;
using TaskManagerAPIPractice.Contracts.Response;
using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserRequest userRequest)
        {
            _logger.LogInformation("Registering new user with email: {Email}", userRequest.Email);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Registration failed for email {Email}: Invalid model state.", userRequest.Email);
                return BadRequest(ModelState);
            }

            try
            {
                await _userService.Register(userRequest.UserName, userRequest.Email, userRequest.Password);
                _logger.LogInformation("User {Email} registered successfully.", userRequest.Email);
                return Ok("User registered successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering user with email {Email}.", userRequest.Email);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest userRequest)
        {
            _logger.LogInformation("Attempting to login user with email: {Email}", userRequest.Email);

            try
            {
                var loginResult = await _userService.Login(userRequest.Email, userRequest.Password);

                if (loginResult == null)
                {
                    _logger.LogWarning("Login failed for user with email {Email}.", userRequest.Email);
                    return Unauthorized();
                }

                Response.Cookies.Append("tasty-cookies", loginResult.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });

                _logger.LogInformation("User {Email} logged in successfully.", userRequest.Email);

                return Ok(new
                {
                    Token = loginResult.Token,
                    UserId = loginResult.UserId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while logging in user with email {Email}.", userRequest.Email);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<ActionResult<UserResponse>> GetUserById(Guid id)
        {
            _logger.LogInformation("Fetching user with ID: {UserId}", id);

            try
            {
                var user = await _userService.GetUserById(id);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", id);
                    return NotFound("User not found.");
                }

                _logger.LogInformation("User with ID {UserId} fetched successfully.", id);
                return Ok(MapToUserResponse(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching user with ID {UserId}.", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("Authomatic")]
        [Authorize]
        public async Task<ActionResult<UserResponse>> GetUserByIdAuthomatic()
        {
            var userId = User.FindFirstValue("userId");
            if (userId == null)
            {
                _logger.LogWarning("Unauthorized access attempt to fetch user.");
                return Unauthorized();
            }

            _logger.LogInformation("Fetching user with ID: {UserId}", userId);

            try
            {
                var user = await _userService.GetUserById(Guid.Parse(userId));
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", userId);
                    return NotFound("User not found.");
                }

                _logger.LogInformation("User with ID {UserId} fetched successfully.", userId);
                return Ok(MapToUserResponse(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching user with ID {UserId}.", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserRequest request)
        {
            _logger.LogInformation("Updating user with ID: {UserId}", id);

            try
            {
                var existingUser = await _userService.GetUserById(id);
                if (existingUser == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", id);
                    return NotFound("User not found.");
                }

                // Оновлення даних користувача
                if (!string.IsNullOrWhiteSpace(request.FullName)) existingUser.FullName = request.FullName;
                if (!string.IsNullOrWhiteSpace(request.Email)) existingUser.Email = request.Email;
                if (request.TeamId.HasValue) existingUser.TeamId = request.TeamId;

                if (request.Tags != null)
                {
                    existingUser.Tags = request.Tags.Select(tagId => new TagEntity { Id = tagId }).ToList();
                }

                await _userService.UpdateUser(existingUser);
                _logger.LogInformation("User with ID {UserId} updated successfully.", id);
                return Ok(MapToUserResponse(existingUser));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user with ID {UserId}.", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            _logger.LogInformation("Deleting user with ID: {UserId}", id);

            try
            {
                var result = await _userService.DeleteUser(id);
                if (!result)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", id);
                    return NotFound("User not found.");
                }

                _logger.LogInformation("User with ID {UserId} deleted successfully.", id);
                return Ok("User deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting user with ID {UserId}.", id);
                return StatusCode(500, "Internal server error");
            }
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