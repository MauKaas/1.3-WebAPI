using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace WebAPI.Tests
{
    public class Environment2DControllerTests
    {
        private readonly Mock<IEnvironment2DRepository> _mockRepo;
        private readonly Environment2DController _controller;

        public Environment2DControllerTests()
        {
            _mockRepo = new Mock<IEnvironment2DRepository>();

            _controller = new Environment2DController(_mockRepo.Object);

            // Fake user simuleren
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task Create_ReturnssBadRequest_WhenUserHas5Environments()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetCountByUserIdAsync("test-user-id"))
                .ReturnsAsync(5);

            var newEnv = new Environment2D { Name = "TestWereld" };

            // Act
            var result = await _controller.Create(newEnv);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Je mag maximaal 5 werelden hebben.", badRequest.Value);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenNameIsTooLong()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetCountByUserIdAsync("test-user-id"))
                .ReturnsAsync(0);

            var newEnv = new Environment2D { Name = "DitIsEenHeelLangeNaamDieMeerDanVijfentwintigKaraktersIs" };

            // Act
            var result = await _controller.Create(newEnv);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Naam moet tussen 1 en 25 karakters zijn.", badRequest.Value);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenNameAlreadyExists()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetCountByUserIdAsync("test-user-id"))
                .ReturnsAsync(1);

            _mockRepo.Setup(r => r.GetAllByUserIdAsync("test-user-id"))
                .ReturnsAsync(new List<Environment2D>
                {
                    new Environment2D { Name = "MijnWereld", UserId = "test-user-id" }
                });

            var newEnv = new Environment2D { Name = "mijnwereld" }; // zelfde naam, andere hoofdletters

            // Act
            var result = await _controller.Create(newEnv);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Je hebt al een wereld met deze naam.", badRequest.Value);
        }
    }
}