namespace MimoBackend_Oct24.API.Models;

public class Lesson
{
    public int LessonId { get; set; }
    public string Title { get; set; }
    public int SortOrder { get; set; }
    public int ChapterId { get; set; }
    public Chapter Chapter { get; set; }
}