using Microsoft.AspNetCore.Mvc;
using MimoBackend_Oct24.API.Models;
using MimoBackend_Oct24.API.Repositories.Interfaces;
using MimoBackend_Oct24.API.Services;

namespace MimoBackend_Oct24.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AchievementsController : ControllerBase
    {
        private readonly IAchievementService _achievementService;
        private readonly IRepository<User> _userRepository;

        public AchievementsController(IAchievementService achievementService, IRepository<User> userRepository)
        {
            _achievementService = achievementService;
            _userRepository = userRepository;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetAchievementsForUser(int userId)
        {
            try
            {
                // Check if the user exists
                var user = await _userRepository.GetByIdAsync(userId); // Assuming you have access to _userRepository
                if (user == null)
                {
                    return NotFound("No user found with this id");
                }
                
                // Fetch achievements for this user
                var achievements = await _achievementService.GetAchievementsForUserAsync(userId);
                if (achievements == null || !achievements.Any())
                {
                    return NotFound("No achievements found for this user.");
                }

                return Ok(achievements);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}