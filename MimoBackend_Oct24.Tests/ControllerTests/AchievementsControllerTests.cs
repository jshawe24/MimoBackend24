using Microsoft.AspNetCore.Mvc;
using Moq;
using MimoBackend_Oct24.API.Controllers;
using MimoBackend_Oct24.API.Models;
using MimoBackend_Oct24.API.Repositories.Interfaces;
using MimoBackend_Oct24.API.Services;
using MimoBackend_Oct24.API.Models.DTOs;

namespace MimoBackend_Oct24.Tests.ControllerTests
{
    [TestFixture]
    public class AchievementsControllerTests
    {
        private AchievementsController _controller;
        private Mock<IAchievementService> _mockAchievementService;
        private Mock<IRepository<User>> _mockUserRepository;

        [SetUp]
        public void Setup()
        {
            _mockAchievementService = new Mock<IAchievementService>();
            _mockUserRepository = new Mock<IRepository<User>>();
            _controller = new AchievementsController(_mockAchievementService.Object, _mockUserRepository.Object);
        }

        [Test]
        public async Task GetAchievementsForUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 1;
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId))!.ReturnsAsync((User)null!);

            // Act
            var result = await _controller.GetAchievementsForUser(userId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.That(notFoundResult.Value, Is.EqualTo("No user found with this id"));
        }

        [Test]
        public async Task GetAchievementsForUser_ReturnsNotFound_WhenNoAchievementsFound()
        {
            // Arrange
            var userId = 1;
            var user = new User { UserId = userId };
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);
            _mockAchievementService.Setup(service => service.GetAchievementsForUserAsync(userId))!
                .ReturnsAsync((List<AchievementDto>)null!); // Simulate no achievements

            // Act
            var result = await _controller.GetAchievementsForUser(userId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.That(notFoundResult.Value, Is.EqualTo("No achievements found for this user."));
        }

        [Test]
        public async Task GetAchievementsForUser_ReturnsOkWithAchievements_WhenAchievementsExist()
        {
            // Arrange
            var userId = 1;
            var user = new User { UserId = userId };
            var achievements = new List<AchievementDto>
            {
                new() { AchievementId = 1, Title = "Achievement 1" },
                new() { AchievementId = 2, Title = "Achievement 2" }
            };

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);
            _mockAchievementService.Setup(service => service.GetAchievementsForUserAsync(userId))
                .ReturnsAsync(achievements);

            // Act
            var result = await _controller.GetAchievementsForUser(userId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.Value, Is.EqualTo(achievements));
        }

        [Test]
        public async Task GetAchievementsForUser_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            var userId = 1;
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ThrowsAsync(new System.Exception("Some error"));

            // Act
            var result = await _controller.GetAchievementsForUser(userId);

            // Assert
            var statusResult = result as ObjectResult;
            Assert.IsNotNull(statusResult);
            Assert.That(statusResult.StatusCode, Is.EqualTo(500));
            Assert.That(statusResult.Value, Is.EqualTo("Internal server error: Some error"));
        }
    }
}
