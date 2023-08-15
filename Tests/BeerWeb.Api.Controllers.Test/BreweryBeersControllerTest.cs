using AutoFixture;
using BeerWeb.Api.DataAccess.Model;
using BeerWeb.Api.Dto;
using BeerWeb.Api.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BeerWeb.Api.Controllers.Test
{
    public class BreweryBeersControllerTest
    {
        private readonly IFixture fixture;
        private readonly Mock<IBreweryBeersService> breweryBeersServiceMock;
        private readonly BreweryBeerController breweryBeersController;
        public BreweryBeersControllerTest()
        {
            fixture = new Fixture();
            breweryBeersServiceMock = fixture.Freeze<Mock<IBreweryBeersService>>();
            breweryBeersController = new BreweryBeerController(breweryBeersServiceMock.Object);
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task GetBreweryBeers_ShouldReturnAllBreweryBeers()
        {
            //Arrange  
            var breweryBeersMock = fixture.CreateMany<BreweryBeersDto>(3).ToList();
            breweryBeersServiceMock.Setup(x => x.GetAllBreweriesWithBeers()).ReturnsAsync(breweryBeersMock);

            //Act
            var result = await breweryBeersController.GetBreweryBeers();

            //Assert
            var okResult = result?.Result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult?.StatusCode);
            var breweryBeerDto = okResult?.Value as IEnumerable<BreweryBeersDto>;
            Assert.Equal(breweryBeersMock.Count(), breweryBeerDto?.Count());
        }

        [Fact]
        public async Task GetBreweryBeerss_ShouldReturnNotFoundResultStatusCode_WhenNoResultFound()
        {
            //Act
            var result = await breweryBeersController.GetBreweryBeers();
            //Assert
            var notFoundResult = result?.Result as NotFoundResult;
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult?.StatusCode);

        }

        [Fact]
        public async Task GetBreweryBeers_ShouldReturnBreweryrBeersForGivenBreweryId_WhenBreweryIdGiven()
        {
            //Arrange  
            var breweryBeersMock = fixture.CreateMany<BreweryBeersDto>(1).ToList();
            var brewery = breweryBeersMock?.FirstOrDefault()?.Brewery;
            if (brewery != null)
                brewery.BreweryId = 25;

            breweryBeersServiceMock.Setup(x => x.GetBrewerybyIdWithAllBeers(It.IsAny<int>())).ReturnsAsync(breweryBeersMock);

            //Act
            var result = await breweryBeersController.GetBreweryBeers(25);

            //Assert
            var okResult = result?.Result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult?.StatusCode);
            var breweryBeerDto = okResult?.Value as IEnumerable<BreweryBeersDto>;
            var expectedBreweryId = 25;
            Assert.Equal(expectedBreweryId, breweryBeerDto?.FirstOrDefault()?.Brewery?.BreweryId);
        }

        [Fact]
        public async Task AddBreweryBeers_ShouldReturnStatusCreated_WhenAddingNewBreweryBeers()
        {
            //Arrange          
            var BreweryBeersMock = fixture.Create<BreweryBeerDto>();
            breweryBeersServiceMock.Setup(x => x.AddBreweryBeer(BreweryBeersMock));

            //Act
            var result = await breweryBeersController.PostBreweryBeer(BreweryBeersMock);

            //Assert
            var okResult = result?.Result as CreatedAtActionResult;
            Assert.Equal(StatusCodes.Status201Created, okResult?.StatusCode);
            var bar = okResult?.Value as BreweryBeerDto;
            Assert.NotNull(bar);
        }
    }
}

