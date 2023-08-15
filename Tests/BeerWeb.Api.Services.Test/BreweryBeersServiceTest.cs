using AutoFixture;
using AutoMapper;
using BeerWeb.Api.DataAccess.DatabaseContext;
using BeerWeb.Api.DataAccess.Interface;
using BeerWeb.Api.DataAccess.Model;
using BeerWeb.Api.Dto;
using BeerWeb.Api.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Moq;
using Serilog;
using System.Linq.Expressions;

namespace BeerWeb.Api.Services.Test
{
    public class BreweryBeersServiceTest
    {
        private readonly IFixture fixture;
        protected readonly BeerStoreDbContext dbContext;
        private readonly ILogger log;
        private readonly IMapper mapper;
        private readonly IBreweryBeersService breweryBeersService;
        private readonly Mock<IUnitOfWork> unitOfWork;

        public BreweryBeersServiceTest()
        {
            fixture = new Fixture();
            var options = new DbContextOptionsBuilder<BeerStoreDbContext>()
           .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            dbContext = new BeerStoreDbContext(options);
            log = new LoggerConfiguration().MinimumLevel.Information().WriteTo.Console().CreateLogger();

            mapper = new Mapper(new MapperConfiguration(x =>
            {
                x.CreateMap<BreweryBeer, BreweryBeerDto>().ReverseMap();
            }));

            unitOfWork = fixture.Freeze<Mock<IUnitOfWork>>();
            unitOfWork.Setup(repo => repo.BreweryBeerRepository).Returns(fixture.Freeze<Mock<IBreweryBeerRepository>>().Object);

            breweryBeersService = new BreweryBeersService(unitOfWork.Object, log, mapper);
            dbContext.Database.EnsureCreated();
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task GetAllBarsWithBeers_ShouldReturnAllBarWithBeers()
        {
            /// Arrange
            var breweryBeersMock = fixture.CreateMany<BreweryBeersDto>(5).ToList();
            unitOfWork.Setup(repo => repo.BreweryBeerRepository.GetAll()).ReturnsAsync(breweryBeersMock);

            /// Act
            var result = await breweryBeersService.GetAllBreweriesWithBeers();

            /// Assert                       
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetBrewerbyIdWithAllBeers_ShouldReturnAllBarWithBeers()
        {
            /// Arrange
            var breweryBeersMock = fixture.CreateMany<BreweryBeersDto>(1).ToList();
            unitOfWork.Setup(repo => repo.BreweryBeerRepository.GetById(1)).ReturnsAsync(breweryBeersMock);

            /// Act
            var result = await breweryBeersService.GetBrewerybyIdWithAllBeers(1);

            /// Assert                       
            Assert.NotNull(result);
            Assert.Equal(breweryBeersMock.Count(), result?.Count());
        }

        [Fact]
        public async Task AddBreweryBeer_ShouldAddNewBreweryBeerLink()
        {
            /// Arrange
            var breweryBeerDtoMock = fixture.Create<BreweryBeerDto>();
            var brewerybeer = mapper.Map<BreweryBeer>(breweryBeerDtoMock);
            unitOfWork.Setup(repo => repo.BreweryBeerRepository.Exists(It.IsAny<int>())).Returns(false);
            unitOfWork.Setup(repo => repo.BreweryRepository.Exists(It.IsAny<Expression<Func<Brewery, bool>>>())).Returns(true);
            unitOfWork.Setup(repo => repo.BeerRepository.Exists(It.IsAny<Expression<Func<Beer, bool>>>())).Returns(true);
            unitOfWork.Setup(repo => repo.BreweryBeerRepository.Add(It.IsAny<BreweryBeer>())).ReturnsAsync(brewerybeer);

            /// Act
            var result = await breweryBeersService.AddBreweryBeer(breweryBeerDtoMock);

            //Assert
            Assert.Equal(breweryBeerDtoMock.BreweryId, result.BreweryId);
            Assert.Equal(breweryBeerDtoMock.BeerId, result.BeerId);
        }
    }
}