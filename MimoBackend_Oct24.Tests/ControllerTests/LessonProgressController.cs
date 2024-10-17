using Microsoft.AspNetCore.Mvc;
using Moq;
using MimoBackend_Oct24.API.Controllers;
using MimoBackend_Oct24.API.Models;
using MimoBackend_Oct24.API.Models.DTOs;
using MimoBackend_Oct24.API.Services;

namespace MimoBackend_Oct24.Tests.ControllerTests;

[TestFixture]
public class LessonProgressControllerTests
{
    private LessonProgressController _controller;
    private Mock<ILessonProgressService> _mockLessonProgressService;
    private Mock<IAchievementService> _mockAchievementService;

    [SetUp]
    public void Setup()
    {
        _mockLessonProgressService = new Mock<ILessonProgressService>();
        _mockAchievementService = new Mock<IAchievementService>();
        _controller = new LessonProgressController(_mockLessonProgressService.Object, _mockAchievementService.Object);
    }

    [Test]
    public async Task CompleteLesson_ReturnsBadRequest_WhenInputIsNull()
    {
        // Act
        var result = await _controller.CompleteLesson(null!);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.That(badRequestResult.Value, Is.EqualTo("Invalid lesson progress data."));
    }

    [Test]
    public async Task CompleteLesson_ReturnsCreated_WhenLessonCompletedSuccessfully() 
    {
        // Arrange
        var lessonProgressDto = new LessonProgressDto 
        { 
            UserId = 1, 
            LessonId = 1, 
            StartTime = DateTime.Now, 
            EndTime = DateTime.Now.AddMinutes(30) 
        };

        // Create the expected result as the new response DTO
        var expectedResult = new LessonProgressResponseDto
        {
            LessonProgressId = 1, // Assuming this is set in the service
            UserId = lessonProgressDto.UserId,
            LessonId = lessonProgressDto.LessonId,
            StartTime = lessonProgressDto.StartTime,
            EndTime = lessonProgressDto.EndTime,
            Status = "Completed" // As per your new DTO definition
        };

        // Setup the mock service to return the new response DTO
        _mockLessonProgressService.Setup(service => service.CompleteLessonAsync(It.IsAny<LessonProgressDto>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.CompleteLesson(lessonProgressDto);

        // Assert
        var createdResult = result as CreatedAtActionResult; 
        Assert.IsNotNull(createdResult); 
        Assert.That(createdResult.ActionName, Is.EqualTo("CompleteLesson")); 
        Assert.That(createdResult.RouteValues?["id"], Is.EqualTo(expectedResult.LessonProgressId)); // Change to use LessonProgressId
        Assert.That(createdResult.Value, Is.EqualTo(expectedResult)); 
    }

    [Test]
    public async Task CompleteLesson_ReturnsBadRequest_WhenArgumentExceptionIsThrown()
    {
        // Arrange
        var lessonProgressDto = new LessonProgressDto { UserId = 1, LessonId = 1};
        _mockLessonProgressService.Setup(service => service.CompleteLessonAsync(lessonProgressDto))
            .ThrowsAsync(new ArgumentException("Invalid lesson ID"));

        // Act
        var result = await _controller.CompleteLesson(lessonProgressDto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.That(badRequestResult.Value, Is.EqualTo("Invalid lesson ID"));
    }

    [Test]
    public async Task CompleteLesson_ReturnsInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var lessonProgressDto = new LessonProgressDto { UserId = 1, LessonId = 1 };
        _mockLessonProgressService.Setup(service => service.CompleteLessonAsync(lessonProgressDto))
            .ThrowsAsync(new Exception("Some error"));

        // Act
        var result = await _controller.CompleteLesson(lessonProgressDto);

        // Assert
        var statusResult = result as ObjectResult;
        Assert.IsNotNull(statusResult);
        Assert.That(statusResult.StatusCode, Is.EqualTo(500));
        Assert.That(statusResult.Value, Is.EqualTo("Internal server error: Some error"));
    }
}
