using MimoBackend_Oct24.API.Models;
using MimoBackend_Oct24.API.Models.DTOs;
using MimoBackend_Oct24.API.Repositories.Interfaces;

namespace MimoBackend_Oct24.API.Services;

public class AchievementService : IAchievementService
{
    private readonly IRepository<UserAchievement> _userAchievementRepository;
    private readonly IRepository<Achievement> _achievementRepository;
    private readonly ILessonProgressService _lessonProgressService;
    private readonly IChapterCompletionService _chapterCompletionService;
    private readonly ICourseCompletionService _courseCompletionService;

    public AchievementService(
        IRepository<UserAchievement> userAchievementRepository,
        IRepository<Achievement> achievementRepository,
        ILessonProgressService lessonProgressService,
        IChapterCompletionService chapterCompletionService,
        ICourseCompletionService courseCompletionService)
    {
        _userAchievementRepository = userAchievementRepository;
        _achievementRepository = achievementRepository;
        _lessonProgressService = lessonProgressService;
        _chapterCompletionService = chapterCompletionService;
        _courseCompletionService = courseCompletionService;
    }

    public async Task UpdateAchievementsAsync(int userId)
    {
        var distinctLessonCount = await _lessonProgressService.CountDistinctLessonsCompletedByUser(userId);
        var completedChapterIds = await _chapterCompletionService.GetCompletedChapterIdsForUser(userId);
        var chapterCount = completedChapterIds.Count;

        var userAchievements = await _userAchievementRepository.GetAllAsync();
        userAchievements = userAchievements.Where(ua => ua.UserId == userId).ToList();

        if (!userAchievements.Any())
        {
            var achievements = await _achievementRepository.GetAllAsync();
            foreach (var achievement in achievements)
            {
                var newUserAchievement = new UserAchievement
                {
                    UserId = userId,
                    AchievementId = achievement.AchievementId,
                    Progress = 0,
                    IsCompleted = false
                };
                await _userAchievementRepository.AddAsync(newUserAchievement);
            }
            
            await _userAchievementRepository.SaveChangesAsync();

            // After saving, get the updated list
            userAchievements = await _userAchievementRepository.GetAllAsync();
            
        }

        foreach (var userAchievement in userAchievements)
        {
            switch (userAchievement.AchievementId)
            {
                case 1:
                    userAchievement.Progress = Math.Min(distinctLessonCount, 5);
                    userAchievement.IsCompleted = distinctLessonCount >= 5;
                    break;
                case 2:
                    userAchievement.Progress = Math.Min(distinctLessonCount, 25);
                    userAchievement.IsCompleted = distinctLessonCount >= 25;
                    break;
                case 3:
                    userAchievement.Progress = Math.Min(distinctLessonCount, 50);
                    userAchievement.IsCompleted = distinctLessonCount >= 50;
                    break;
                case 4:
                    userAchievement.Progress = Math.Min(chapterCount, 1);
                    userAchievement.IsCompleted = chapterCount >= 1;
                    break;
                case 5:
                    userAchievement.Progress = Math.Min(chapterCount, 5);
                    userAchievement.IsCompleted = chapterCount >= 5;
                    break;
                case 6:
                    userAchievement.Progress = await _courseCompletionService.CheckCompletedCourseByLessons(userId, 1);
                    userAchievement.IsCompleted = userAchievement.Progress == 1;
                    break;
                case 7:
                    userAchievement.Progress = await _courseCompletionService.CheckCompletedCourseByLessons(userId, 2);
                    userAchievement.IsCompleted = userAchievement.Progress == 1;
                    break;
                case 8:
                    userAchievement.Progress = await _courseCompletionService.CheckCompletedCourseByLessons(userId, 3);
                    userAchievement.IsCompleted = userAchievement.Progress == 1;
                    break;
            }

            _userAchievementRepository.Update(userAchievement);
        }

        await _userAchievementRepository.SaveChangesAsync();
    }
    
    public async Task<List<AchievementDto>> GetAchievementsForUserAsync(int userId)
    {
        // Fetch all user achievements
        var userAchievements = await _userAchievementRepository.GetAllAsync();
        userAchievements = userAchievements.Where(ua => ua.UserId == userId).ToList();

        // If there are no achievements for the user, return null
        if (!userAchievements.Any())
        {
            return null;
        }

        // Map UserAchievement to AchievementDto
        var achievementsDto = new List<AchievementDto>();
        foreach (var userAchievement in userAchievements)
        {
            var achievement = await _achievementRepository.GetByIdAsync(userAchievement.AchievementId);
            var achievementDto = new AchievementDto
            {
                AchievementId = achievement.AchievementId,
                Title = achievement.Title,
                IsCompleted = userAchievement.IsCompleted,
                Progress = userAchievement.Progress
            };
            achievementsDto.Add(achievementDto);
        }

        return achievementsDto;
    }
}