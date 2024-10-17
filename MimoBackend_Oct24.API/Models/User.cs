using Newtonsoft.Json;

namespace MimoBackend_Oct24.API.Models;

public class User
{
    public int UserId { get; set; }
    public string UserName { get; set; }
    
    [JsonIgnore]
    public ICollection<LessonProgress> LessonProgresses { get; set; }
    public ICollection<UserAchievement> UserAchievements { get; set; }
}