using AutoFixture;
using BeerWeb.Api.Dto;
using BeerWeb.Api.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BeerWeb.Api.Controllers.Test
{
    public class BarBeersBeersControllerTest
    {
        private readonly IFixture fixture;
        private readonly Mock<IBarBeersService> barBeersServiceMock;
        private readonly BarBeerController barBeersController;
        public BarBeersBeersControllerTest()
        {
            fixture = new Fixture();
            barBeersServiceMock = fixture.Freeze<Mock<IBarBeersService>>();
            barBeersController = new BarBeerController(barBeersServiceMock.Object);
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task GetBarBeers_ShouldReturnAllBarBeers()
        {
            //Arrange  
            var barBeersMock = fixture.CreateMany<BarBeersDto>(3).ToList();
            barBeersServiceMock.Setup(x => x.GetAllBarsWithBeers()).ReturnsAsync(barBeersMock);

            //Act
            var result = await barBeersController.GetBarBeers();

            //Assert
            var okResult = result?.Result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult?.StatusCode);
            var barBeerDto = okResult?.Value as IEnumerable<BarBeersDto>;
            Assert.Equal(barBeersMock.Count(), barBeerDto?.Count());
        }

        [Fact]
        public async Task GetBarBeers_ShouldReturnNotFoundResultStatusCode_WhenNoResultFound()
        {
            //Act
            var result = await barBeersController.GetBarBeers();
            //Assert
            var notFoundResult = result?.Result as NotFoundResult;
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult?.StatusCode);
        }

        [Fact]
        public async Task GetBarBeers_ShouldReturnBarBeersForGivenBarId_WhenBarIdGiven()
        {
            //Arrange  
            var barBeersMock = fixture.CreateMany<BarBeersDto>(1).ToList();
            var bar = barBeersMock?.FirstOrDefault()?.Bar;
            
            if(bar!=null) 
                bar.BarId = 10;

            barBeersServiceMock.Setup(x => x.GetBarbyIdWithAllBeers(It.IsAny<int>())).ReturnsAsync(barBeersMock);

            //Act
            var result = await barBeersController.GetBarBeers(10);

            //Assert
            var okResult = result?.Result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult?.StatusCode);
            var barBeerDto = okResult?.Value as IEnumerable<BarBeersDto>;
            var expectedBarId = 10;
            Assert.Equal(expectedBarId, barBeerDto?.FirstOrDefault()?.Bar?.BarId);
        }

        [Fact]
        public async Task AddBarBeers_ShouldReturnStatusCreated_WhenAddingNewBarBeers()
        {
            //Arrange          
            var BarBeersMock = fixture.Create<BarBeerDto>();
            barBeersServiceMock.Setup(x => x.AddBarBeer(BarBeersMock));

            //Act
            var result = await barBeersController.PostBarBeer(BarBeersMock);
            
            //Assert
            var okResult = result?.Result as CreatedAtActionResult;
            Assert.Equal(StatusCodes.Status201Created, okResult?.StatusCode);
            var bar = okResult?.Value as BarBeerDto;
            Assert.NotNull(bar);
        }
    }
}

