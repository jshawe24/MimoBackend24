namespace MimoBackend_Oct24.API.Models;

public class Course
{
    public int CourseId { get; set; }
    public string Title { get; set; }
    public ICollection<Chapter> Chapters { get; set; }
}