namespace MimoBackend_Oct24.API.Models;

public class LessonProgress
{
    public int LessonProgressId { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }

    public int LessonId { get; set; }
    public Lesson Lesson { get; set; }

    public int ChapterId { get; set; }  // New field
    public Chapter Chapter { get; set; }

    public int CourseId { get; set; }  // New field
    public Course Course { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}