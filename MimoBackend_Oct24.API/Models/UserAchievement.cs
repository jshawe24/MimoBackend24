namespace MimoBackend_Oct24.API.Models;

public class UserAchievement
{
    public int UserAchievementId { get; set; }
    public int UserId { get; set; }
    public int AchievementId { get; set; }
    public bool IsCompleted { get; set; }
    public int Progress { get; set; } // Track progress toward achievement
    public User User { get; set; }
    public Achievement Achievement { get; set; }
}