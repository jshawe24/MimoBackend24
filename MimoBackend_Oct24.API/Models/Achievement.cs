namespace MimoBackend_Oct24.API.Models;

public class Achievement
{
    public int AchievementId { get; set; }
    public string Title { get; set; }
    public int Target { get; set; } // e.g., 5 lessons, 1 chapter
    public ICollection<UserAchievement> UserAchievements { get; set; }
}