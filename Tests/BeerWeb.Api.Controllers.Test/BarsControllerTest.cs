using AutoFixture;
using BeerWeb.Api.Dto;
using BeerWeb.Api.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BeerWeb.Api.Controllers.Test
{
    public class BarsControllerTest
    {
        private readonly IFixture fixture;
        private readonly Mock<IBarService> barServiceMock;
        private readonly BarController barController;
        public BarsControllerTest()
        {
            fixture = new Fixture();
            barServiceMock = fixture.Freeze<Mock<IBarService>>();
            barController = new BarController(barServiceMock.Object);
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task GetBars_ShouldReturnOkStatus_WithAllBars()
        {
            //Arrange  
            var barMock = fixture.CreateMany<BarDto>(3).ToList();
            barServiceMock.Setup(x => x.GetAll()).ReturnsAsync(barMock);

            //Act
            var result = await barController.GetBars();

            //Assert
            var okResult = result?.Result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult?.StatusCode);
            var bars = okResult?.Value as IEnumerable<BarDto>;
            Assert.Equal(barMock.Count(), bars?.Count());
        }

        [Fact]
        public async Task GetBars_ShouldReturnNotFoundStatus_WhenNoResultFound()
        {
            //Act
            var result = await barController.GetBars();
            //Assert
            var notFoundResult = result?.Result as NotFoundResult;
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult?.StatusCode);
        }

        [Fact]
        public async Task GetBar_ShouldReturnOkStatus_WithBar()
        {
            //Arrange
            var barId = 100;
            var mockBarDto = fixture.Build<BarDto>()
                .With(x => x.BarId, barId)
                .Create();
            barServiceMock.Setup(x => x.GetById(barId)).ReturnsAsync(mockBarDto);

            //Act
            var result = await barController.GetBar(barId);

            //Assert
            var okResult = result?.Result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult?.StatusCode);
            var bar = okResult?.Value as BarDto;
            Assert.Equal(barId, bar?.BarId);
        }

       [Fact]
        public async Task GetBar_ShouldReturnNotFoundStatus_WhenNoBarForGivenId()
        {
            //Act
            var result = await barController.GetBar(30);
            //Assert
            var notFoundResult = result?.Result as NotFoundResult;
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult?.StatusCode);
        }

        [Fact]
        public async Task AddBar_ShouldReturnStatusCreated_WhenAddingNewBar()
        {
            //Arrange
            var barMock = fixture.Create<BarDto>();
            barServiceMock.Setup(x => x.AddBar(barMock));

            //Act
            var result = await barController.AddBar(barMock);

            //Assert
            var okResult = result?.Result as CreatedAtActionResult;
            Assert.Equal(StatusCodes.Status201Created, okResult?.StatusCode);
            var bar = okResult?.Value as BarDto;
            Assert.NotNull(bar);
        }

        [Fact]
        public async Task AddBar_ShouldReturnError_WhenAddingNewBar()
        {
            //Arrange
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var barMock = fixture.Create<BarDto>();
            barMock.BarId = 0;
            barServiceMock.Setup(x => x.AddBar(barMock));

            //Act
            var result = await barController.AddBar(barMock);

            //Assert
            var badRequest = result?.Result as BadRequestResult;
            Assert.Equal(StatusCodes.Status400BadRequest, badRequest?.StatusCode);
        }

       [Fact]
        public async Task UpdateBar_ShouldReturnStatusOK_WhenUpdatingBar()
        {
            //Arrange
            fixture.Register(() => 23);
            int id = fixture.Create<int>();
            var barMock = fixture.Build<BarDto>()
                .With(x => x.BarId, 23).Without(n => n.BarId)
                .Create();
            barServiceMock.Setup(x => x.UpdateBar(id, barMock));

            //Act
            var result = await barController.UpdateBar(id, barMock) as OkResult;

            //Assert  
            Assert.Equal(StatusCodes.Status200OK, result?.StatusCode);
        }

        [Fact]
        public async Task UpdateBar_ShouldReturnStatusNotFound_WhenUpdatingBar()
        {
            //Arrange
            fixture.Register(() => 100);
            int id = fixture.Create<int>();
            var barMock = fixture.Build<BarDto>()
                .With(x => x.BarId, id).Without(n => n.BarId)
                .Create();
            barServiceMock.Setup(x => x.UpdateBar(id, barMock));

            //Act
            var result = await barController.UpdateBar(0, barMock) as BadRequestResult;

            //Assert  
            Assert.Equal(StatusCodes.Status400BadRequest, result?.StatusCode);
        }
    }
}
