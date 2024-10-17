using Moq;
using MimoBackend_Oct24.API.Models;
using MimoBackend_Oct24.API.Models.DTOs;
using MimoBackend_Oct24.API.Repositories.Interfaces;
using MimoBackend_Oct24.API.Services;

namespace MimoBackend_Oct24.Tests.ServiceTests.LessonProgressServiceTests;

[TestFixture]
public class ValidateLessonProgressAsyncTests
{
    private Mock<IRepository<User>> _userRepository;
    private Mock<ILessonRepository> _lessonRepository;
    private Mock<ILessonProgressRepository> _lessonProgressRepository;
    private LessonProgressService _service;

    [SetUp]
    public void SetUp()
    {
        _userRepository = new Mock<IRepository<User>>();
        _lessonRepository = new Mock<ILessonRepository>();
        _lessonProgressRepository = new Mock<ILessonProgressRepository>();

        // Initialize the LessonProgressService with mocked repositories
        _service = new LessonProgressService(
            _userRepository.Object,
            _lessonRepository.Object,
            _lessonProgressRepository.Object
        );
    }

    [Test]
    public async Task ValidateLessonProgressAsync_WhenLessonDoesNotExist_ReturnsInvalidResult()
    {
        // Arrange: Set up the mock to return null for a specific lesson ID.
        var dto = new LessonProgressDto { LessonId = 1, UserId = 1, StartTime = DateTime.Now, EndTime = DateTime.Now.AddMinutes(1) };
        _lessonRepository.Setup(repo => repo.GetByIdAsync(dto.LessonId))!.ReturnsAsync((Lesson)null!);

        // Act: Call the ValidateLessonProgressAsync method of the service, to validate the dto values.
        var result = await _service.ValidateLessonProgressAsync(dto);

        // Assert: Verify that the result indicates the lesson was not found and it returns correct error message.
        Assert.IsFalse(result.IsValid);
        Assert.That(result.ErrorMessage, Is.EqualTo("Lesson not found."));
    }

    [Test]
    public async Task ValidateLessonProgressAsync_WhenUserDoesNotExist_ReturnsInvalidResult()
    {
        // Arrange: Set up the mock to return a valid lesson but null user.
        var dto = new LessonProgressDto { LessonId = 1, UserId = 1, StartTime = DateTime.Now, EndTime = DateTime.Now.AddMinutes(1) };

        _lessonRepository.Setup(repo => repo.GetByIdAsync(dto.LessonId)).ReturnsAsync(new Lesson());
        _userRepository.Setup(repo => repo.GetByIdAsync(dto.UserId))!.ReturnsAsync((User)null!);

        // Act: Call the ValidateLessonProgressAsync method of the service.
        var result = await _service.ValidateLessonProgressAsync(dto);

        // Assert: Verify that the result indicates the user was not found.
        Assert.IsFalse(result.IsValid);
        Assert.That(result.ErrorMessage, Is.EqualTo("User not found."));
    }

    [Test]
    public async Task ValidateLessonProgressAsync_WhenStartTimeIsAfterCompletionTime_ReturnsInvalidResult()
    {
        // Arrange: Set up the mock to return valid lesson and user, but with invalid times.
        var dto = new LessonProgressDto { LessonId = 1, UserId = 1, StartTime = DateTime.Now.AddMinutes(1), EndTime = DateTime.Now };

        _lessonRepository.Setup(repo => repo.GetByIdAsync(dto.LessonId)).ReturnsAsync(new Lesson());
        _userRepository.Setup(repo => repo.GetByIdAsync(dto.UserId)).ReturnsAsync(new User());

        // Act: Call the ValidateLessonProgressAsync method of the service.
        var result = await _service.ValidateLessonProgressAsync(dto);

        // Assert: Verify that the result indicates the start time is after the end time.
        Assert.IsFalse(result.IsValid);
        Assert.That(result.ErrorMessage, Is.EqualTo("Start time must be before completion time."));
    }

    [Test]
    public async Task ValidateLessonProgressAsync_WhenValid_ReturnsValidResult()
    {
        // Arrange: Set up the mock to return valid lesson and user with valid times.
        var dto = new LessonProgressDto { LessonId = 1, UserId = 1, StartTime = DateTime.Now, EndTime = DateTime.Now.AddMinutes(1) };

        _lessonRepository.Setup(repo => repo.GetByIdAsync(dto.LessonId)).ReturnsAsync(new Lesson());
        _userRepository.Setup(repo => repo.GetByIdAsync(dto.UserId)).ReturnsAsync(new User());

        // Act: Call the ValidateLessonProgressAsync method of the service.
        var result = await _service.ValidateLessonProgressAsync(dto);

        // Assert: Verify that the result is valid.
        Assert.IsTrue(result.IsValid);
        Assert.IsNull(result.ErrorMessage);
    }
}
