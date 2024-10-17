using Microsoft.EntityFrameworkCore;
using MimoBackend_Oct24.API.Models;

namespace MimoBackend_Oct24.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<LessonProgress> LessonProgresses { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<UserAchievement> UserAchievements { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Course> Courses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Composite Key for UserAchievement
            modelBuilder.Entity<UserAchievement>()
                .HasKey(ua => new { ua.UserId, ua.AchievementId });

            base.OnModelCreating(modelBuilder);

            // Seed Courses
            modelBuilder.Entity<Course>().HasData(
                new Course { CourseId = 1, Title = "Swift" },
                new Course { CourseId = 2, Title = "Javascript" },
                new Course { CourseId = 3, Title = "C#" }
            );

            // Seed Chapters
            modelBuilder.Entity<Chapter>().HasData(
                new Chapter { ChapterId = 1, Title = "Swift Basics", CourseId = 1 },
                new Chapter { ChapterId = 2, Title = "Swift Advanced", CourseId = 1 },
                new Chapter { ChapterId = 3, Title = "Javascript Basics", CourseId = 2 },
                new Chapter { ChapterId = 4, Title = "Javascript Advanced", CourseId = 2 },
                new Chapter { ChapterId = 5, Title = "C# Basics", CourseId = 3 },
                new Chapter { ChapterId = 6, Title = "C# Advanced", CourseId = 3 }
            );

            // Seed Lessons
            modelBuilder.Entity<Lesson>().HasData(
                new Lesson { LessonId = 1, Title = "Swift Basics Lesson 1", ChapterId = 1 },
                new Lesson { LessonId = 2, Title = "Swift Basics Lesson 2", ChapterId = 1 },
                new Lesson { LessonId = 3, Title = "Swift Advanced Lesson 1", ChapterId = 2 },
                new Lesson { LessonId = 4, Title = "Swift Advanced Lesson 2", ChapterId = 2 },
                new Lesson { LessonId = 5, Title = "Javascript Basics Lesson 1", ChapterId = 3 },
                new Lesson { LessonId = 6, Title = "Javascript Basics Lesson 2", ChapterId = 3 },
                new Lesson { LessonId = 7, Title = "Javascript Advanced Lesson 1", ChapterId = 4 },
                new Lesson { LessonId = 8, Title = "Javascript Advanced Lesson 2", ChapterId = 4 },
                new Lesson { LessonId = 9, Title = "C# Basics Lesson 1", ChapterId = 5 },
                new Lesson { LessonId = 10, Title = "C# Basics Lesson 2", ChapterId = 5 },
                new Lesson { LessonId = 11, Title = "C# Advanced Lesson 1", ChapterId = 6 },
                new Lesson { LessonId = 12, Title = "C# Advanced Lesson 2", ChapterId = 6 }
            );

            // Seed Achievements
            modelBuilder.Entity<Achievement>().HasData(
                new Achievement { AchievementId = 1, Title = "Complete 5 Lessons", Target = 5 },
                new Achievement { AchievementId = 2, Title = "Complete 25 Lessons", Target = 25 },
                new Achievement { AchievementId = 3, Title = "Complete 50 Lessons", Target = 50 },
                new Achievement { AchievementId = 4, Title = "Complete 1 Chapter", Target = 1 },
                new Achievement { AchievementId = 5, Title = "Complete 5 Chapters", Target = 5 },
                new Achievement { AchievementId = 6, Title = "Complete Swift Course", Target = 1 },  
                new Achievement { AchievementId = 7, Title = "Complete Javascript Course", Target = 1 },  
                new Achievement { AchievementId = 8, Title = "Complete C# Course", Target = 1 }  
            );

            // Seed Users
            modelBuilder.Entity<User>().HasData(
                new User { UserId = 1, UserName = "John" },
                new User { UserId = 2, UserName = "Paul" },
                new User { UserId = 3, UserName = "Jane" },
                new User { UserId = 4, UserName = "Sarah" }
            );
        }
    }
}
