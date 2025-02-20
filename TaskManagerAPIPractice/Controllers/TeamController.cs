using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagerAPIPractice.Contracts.Response;
using TaskManagerAPIPractice.Contracts;

namespace TaskManagerAPIPractice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TeamController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TeamController> _logger;

        public TeamController(IMediator mediator, ILogger<TeamController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("idUser")]
        public async Task<ActionResult<List<TaskResponse>>> GetByIdAutomaticTeam()
        {
            var userId = User.FindFirstValue("userId");
            if (userId == null)
            {
                _logger.LogWarning("Unauthorized access attempt to GetByIdAutomaticTeam.");
                return Unauthorized();
            }

            _logger.LogInformation("Fetching tasks for user {UserId}", userId);
            var response = await _mediator.Send(new GetUserTeamsQuery(Guid.Parse(userId)));
            return Ok(response);
        }
    }
}
