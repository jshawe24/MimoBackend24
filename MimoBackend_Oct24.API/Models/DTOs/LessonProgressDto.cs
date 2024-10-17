namespace MimoBackend_Oct24.API.Models.DTOs;

public class LessonProgressDto
{
    public int UserId { get; set; }
    public int LessonId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}