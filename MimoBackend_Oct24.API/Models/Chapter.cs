namespace MimoBackend_Oct24.API.Models;

public class Chapter
{
    public int ChapterId { get; set; }
    public string Title { get; set; }
    public int SortOrder { get; set; }
    public int CourseId { get; set; }
    public Course Course { get; set; }
    public ICollection<Lesson> Lessons { get; set; }
}