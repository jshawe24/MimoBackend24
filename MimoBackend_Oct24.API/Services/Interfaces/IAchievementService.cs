using MimoBackend_Oct24.API.Models.DTOs;

namespace MimoBackend_Oct24.API.Services;

public interface IAchievementService
{
    Task UpdateAchievementsAsync(int userId);
    Task<List<AchievementDto>> GetAchievementsForUserAsync(int userId);
}