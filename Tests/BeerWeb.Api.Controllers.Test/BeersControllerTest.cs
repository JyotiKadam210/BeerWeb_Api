using AutoFixture;
using BeerWeb.Api.Dto;
using BeerWeb.Api.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BeerWeb.Api.Controllers.Test
{
    public class BeersControllerTest
    {
        private readonly IFixture fixture;
        private readonly Mock<IBeerService> beerServiceMock;
        private readonly BeerController beerController;

        public BeersControllerTest()
        {
            fixture = new Fixture();
            beerServiceMock = fixture.Freeze<Mock<IBeerService>>();
            beerController = new BeerController(beerServiceMock.Object);
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task GetBeers_ShouldReturnOkStatus_WithAllBeers()
        {
            //Arrange  
            var beerMock = fixture.CreateMany<BeerDto>(3).ToList();
            beerServiceMock.Setup(x => x.GetBeer(0, 100)).ReturnsAsync(beerMock);

            //Act
            var result = await beerController.GetBeers(0, 100);

            //Assert
            var okResult = result?.Result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult?.StatusCode);
            var beers = okResult?.Value as IEnumerable<BeerDto>;
            Assert.Equal(beerMock.Count(), beers?.Count());
        }

        [Fact]
        public async Task GetBeers_ShouldReturnNotFoundStatus_WhenNoResultFound()
        {
            //Act
            var result = await beerController.GetBeers(0, 100);
            //Assert
            var notFoundResult = result?.Result as NotFoundResult;
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult?.StatusCode);
        }

       [Fact]
        public async Task GetBeer_ShouldReturnOkStatus_WithBeer()
        {
            //Arrange
            var beerId = 100;
            var mockBeerDto = fixture.Build<BeerDto>()
                .With(x => x.BeerId, beerId)
                .Create();
            beerServiceMock.Setup(x => x.GetById(beerId)).ReturnsAsync(mockBeerDto);

            //Act
            var result = await beerController.GetBeer(beerId);

            //Assert
            var okResult = result?.Result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult?.StatusCode);
            var beer = okResult?.Value as BeerDto;
            Assert.Equal(beerId, beer?.BeerId);
        }

       [Fact]
        public async Task GetBeer_ShouldReturnNotFoundStatus_WhenNoBeerForGivenId()
        {
            //Act
            var result = await beerController.GetBeer(12);
            //Assert
            var notFoundResult = result?.Result as NotFoundResult;
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult?.StatusCode);
        }

        [Fact]
        public async Task AddBeer_ShouldReturnStatusCreated_WhenAddingNewBeer()
        {
            //Arrange
            var beerMock = fixture.Create<BeerDto>();
            beerServiceMock.Setup(x => x.AddBeer(beerMock));

            //Act
            var result = await beerController.AddBeer(beerMock);

            //Assert
            var okResult = result?.Result as CreatedAtActionResult;
            Assert.Equal(StatusCodes.Status201Created, okResult?.StatusCode);
            var beer = okResult?.Value as BeerDto;
            Assert.NotNull(beer);
        }

        [Fact]
        public async Task AddBeer_ShouldReturnError_WhenAddingNewBeer()
        {
            //Arrange
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var beerMock = fixture.Create<BeerDto>();
            beerMock.BeerId = 0;
            beerServiceMock.Setup(x => x.AddBeer(beerMock));

            //Act
            var result = await beerController.AddBeer(beerMock);

            //Assert
            var badRequest = result?.Result as BadRequestResult;
            Assert.Equal(StatusCodes.Status400BadRequest, badRequest?.StatusCode);
        }

        [Fact]
        public async Task UpdateBeer_ShouldReturnStatusOK_WhenUpdatingBeer()
        {
            //Arrange
            var id = 10;
            fixture.Register(() => id);
            int beerId = fixture.Create<int>();
            var beerMock = fixture.Build<BeerDto>()
                .With(x => x.BeerId, id).Without(n => n.BeerId)
                .Create();
            beerServiceMock.Setup(x => x.UpdateBeer(beerId, beerMock));

            //Act
            var result = await beerController.UpdateBeer(beerId, beerMock) as OkResult;

            //Assert  
            Assert.Equal(StatusCodes.Status200OK, result?.StatusCode);
        }

       [Fact]
        public async Task UpdateBeer_ShouldReturnStatusNotFound_WhenUpdatingBeer()
        {
            //Arrange
            fixture.Register(() => 100);
            int id = fixture.Create<int>();
            var beerMock = fixture.Build<BeerDto>()
                .With(x => x.BeerId, id).Without(n => n.BeerId)
                .Create();
            beerServiceMock.Setup(x => x.UpdateBeer(id, beerMock));

            //Act
            var result = await beerController.UpdateBeer(0, beerMock) as BadRequestResult;

            //Assert  
            Assert.Equal(StatusCodes.Status400BadRequest, result?.StatusCode);
        }


    }
}


