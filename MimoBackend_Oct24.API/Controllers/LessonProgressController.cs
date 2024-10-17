using Microsoft.AspNetCore.Mvc;
using MimoBackend_Oct24.API.Models.DTOs;
using MimoBackend_Oct24.API.Services;

namespace MimoBackend_Oct24.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonProgressController : ControllerBase
    {
        private readonly ILessonProgressService _lessonProgressService;
        private readonly IAchievementService _achievementService;

        public LessonProgressController(
            ILessonProgressService lessonProgressService,
            IAchievementService achievementService)
        {
            _lessonProgressService = lessonProgressService;
            _achievementService = achievementService;
        }

        [HttpPost("CompleteLesson")]
        public async Task<IActionResult> CompleteLesson([FromBody] LessonProgressDto lessonProgressDto)
        {
            if (lessonProgressDto == null)
            {
                return BadRequest("Invalid lesson progress data.");
            }

            try
            {
                var result = await _lessonProgressService.CompleteLessonAsync(lessonProgressDto);
                
                // Update user achievements based on this lesson progress
                await _achievementService.UpdateAchievementsAsync(result.UserId);
                
                return CreatedAtAction(nameof(CompleteLesson), new { id = result.LessonId }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}