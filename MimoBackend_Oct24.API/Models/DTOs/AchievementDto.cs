namespace MimoBackend_Oct24.API.Models.DTOs;

public class AchievementDto
{
    public int AchievementId { get; set; }
    public string Title { get; set; }
    public bool IsCompleted { get; set; }
    public int Progress { get; set; }
}