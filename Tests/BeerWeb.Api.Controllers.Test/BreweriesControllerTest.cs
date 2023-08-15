using AutoFixture;
using BeerWeb.Api.Dto;
using BeerWeb.Api.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
namespace BeerWeb.Api.Controllers.Test
{
    public class BreweriesControllerTest
    {
        private readonly IFixture fixture;
        private readonly Mock<IBreweryService> breweryServiceMock;
        private readonly BreweryController breweryController;

        public BreweriesControllerTest()
        {
            fixture = new Fixture();
            breweryServiceMock = fixture.Freeze<Mock<IBreweryService>>();
            breweryController = new BreweryController(breweryServiceMock.Object);
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task GetBrewerys_ShouldReturnOkStatus_WithAllBrewerys()
        {
            //Arrange  
            var breweryMock = fixture.CreateMany<BreweryDto>(3).ToList();
            breweryServiceMock.Setup(x => x.GetAll()).ReturnsAsync(breweryMock);

            //Act
            var result = await breweryController.GetBreweries();

            //Assert
            var okResult = result?.Result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult?.StatusCode);
            var brewerys = okResult?.Value as IEnumerable<BreweryDto>;
            Assert.Equal(breweryMock.Count(), brewerys?.Count());
        }

        [Fact]
        public async Task GetBrewerys_ShouldReturnNotFoundStatus_WhenNoResultFound()
        {
            //Act
            var result = await breweryController.GetBreweries();
            //Assert
            var notFoundResult = result?.Result as NotFoundResult;
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult?.StatusCode);
        }

        [Fact]
        public async Task GetBrewery_ShouldReturnOkStatus_WithBrewery()
        {
            //Arrange
            var breweryId = 100;
            var mockBreweryDto = fixture.Build<BreweryDto>()
                .With(x => x.BreweryId, breweryId)
                .Create();
            breweryServiceMock.Setup(x => x.GetById(breweryId)).ReturnsAsync(mockBreweryDto);

            //Act
            var result = await breweryController.GetBrewery(breweryId);

            //Assert
            var okResult = result?.Result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult?.StatusCode);
            var brewery = okResult?.Value as BreweryDto;
            Assert.Equal(breweryId, brewery?.BreweryId);
        }

        [Fact]
        public async Task GetBrewery_ShouldReturnNotFoundStatus_WhenNoBreweryForGivenId()
        {
            //Act
            var result = await breweryController.GetBrewery(10);
            //Assert
            var notFoundResult = result?.Result as NotFoundResult;
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult?.StatusCode);
        }

        [Fact]
        public async Task AddBrewery_ShouldReturnStatusCreated_WhenAddingNewBrewery()
        {
            //Arrange
            var breweryMock = fixture.Create<BreweryDto>();
            breweryServiceMock.Setup(x => x.AddBrewery(breweryMock));

            //Act
            var result = await breweryController.AddBrewery(breweryMock);

            //Assert
            var okResult = result?.Result as CreatedAtActionResult;
            Assert.Equal(StatusCodes.Status201Created, okResult?.StatusCode);
            var brewery = okResult?.Value as BreweryDto;
            Assert.NotNull(brewery);
        }

        [Fact]
        public async Task AddBrewery_ShouldReturnError_WhenAddingNewBrewery()
        {
            //Arrange
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var breweryMock = fixture.Create<BreweryDto>();
            breweryMock.BreweryId = 0;
            breweryServiceMock.Setup(x => x.AddBrewery(breweryMock));

            //Act
            var result = await breweryController.AddBrewery(breweryMock);

            //Assert
            var badRequest = result?.Result as BadRequestResult;
            Assert.Equal(StatusCodes.Status400BadRequest, badRequest?.StatusCode);
        }

       [Fact]
        public async Task UpdateBrewery_ShouldReturnStatusOK_WhenUpdatingBrewery()
        {
            //Arrange
            fixture.Register(() => 50);
            int id = fixture.Create<int>();
            var breweryMock = fixture.Build<BreweryDto>()
                .With(x => x.BreweryId, id).Without(n => n.BreweryId)
                .Create();
            breweryServiceMock.Setup(x => x.UpdateBrewery(id, breweryMock));

            //Act
            var result = await breweryController.UpdateBrewery(id, breweryMock) as OkResult;

            //Assert  
            Assert.Equal(StatusCodes.Status200OK, result?.StatusCode);
        }

        [Fact]
        public async Task UpdateBrewery_ShouldReturnStatusNotFound_WhenUpdatingBrewery()
        {
            //Arrange
            fixture.Register(() => 100);
            int id = fixture.Create<int>();
            var breweryMock = fixture.Build<BreweryDto>()
                .With(x => x.BreweryId, id).Without(n => n.BreweryId)
                .Create();
            breweryServiceMock.Setup(x => x.UpdateBrewery(id, breweryMock));

            //Act
            var result = await breweryController.UpdateBrewery(0, breweryMock) as BadRequestResult;

            //Assert  
            Assert.Equal(StatusCodes.Status400BadRequest, result?.StatusCode);
        }
    }
}