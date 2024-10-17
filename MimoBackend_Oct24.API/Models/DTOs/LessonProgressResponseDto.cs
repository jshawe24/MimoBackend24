namespace MimoBackend_Oct24.API.Models.DTOs;

public class LessonProgressResponseDto
{
    public int LessonProgressId { get; set; }
    public int LessonId { get; set; }
    public int UserId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = "Completed"; // Default status
}