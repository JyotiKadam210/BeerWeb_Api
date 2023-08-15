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
    public class BarBeerServiceTest
    {
        private readonly IFixture fixture;
        protected readonly BeerStoreDbContext dbContext;
        private readonly ILogger log;
        private readonly Mock<IUnitOfWork> unitOfWork ;
        private readonly IMapper mapper;
        private readonly IBarBeersService barBeerService;

        public BarBeerServiceTest()
        {
            fixture = new Fixture();
            var options = new DbContextOptionsBuilder<BeerStoreDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            dbContext = new BeerStoreDbContext(options);
            dbContext.Database.EnsureCreated();
            unitOfWork = fixture.Freeze<Mock<IUnitOfWork>>();
            unitOfWork.Setup(repo => repo.BarBeerRepository).Returns(fixture.Freeze<Mock<IBarBeerRepository>>().Object);

            mapper = new Mapper(new MapperConfiguration(x =>
            {

                x.CreateMap<BarBeerDto, BarBeersDto>().ReverseMap();
                x.CreateMap<BarBeer, BarBeerDto>().ReverseMap();

            }));
            log = new LoggerConfiguration().MinimumLevel.Information().WriteTo.Console().CreateLogger();

            barBeerService = new BarBeerService(unitOfWork.Object, log, mapper);
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task GetAllBarsWithBeers_ShouldReturnAllBarWithBeersd()
        {
            /// Arrange
            var mockBarBeer = fixture.CreateMany<BarBeersDto>(5).ToList();
            unitOfWork.Setup(repo => repo.BarBeerRepository.GetAll()).ReturnsAsync(mockBarBeer);

            /// Act
            var result = await barBeerService.GetAllBarsWithBeers();

            /// Assert                     
            Assert.NotNull(result);
            Assert.Equal(mockBarBeer.Count(), result.Count());
        }

        [Fact]
        public async Task GetBarbyIdWithAllBeers_ShouldReturnAllBarWithBeersd()
        {
            /// Arrange
            var barBeerMock = fixture.CreateMany<BarBeersDto>(1).ToList();
            unitOfWork.Setup(repo => repo.BarBeerRepository.GetById(1)).ReturnsAsync(barBeerMock);

            /// Act
            var result = await barBeerService.GetBarbyIdWithAllBeers(1);

            /// Assert                       
            Assert.NotNull(result);
            Assert.Equal(barBeerMock.Count(), result?.Count());
        }

        [Fact]
        public async Task AddBarBeer_ShouldAddNewBarBeerLink()
        {
            /// Arrange
            var barBeerDtoMock = fixture.Create<BarBeerDto>();
            barBeerDtoMock.BeerId = 100;
            barBeerDtoMock.BarId = 200;
            var barbeer = mapper.Map<BarBeer>(barBeerDtoMock);           

            unitOfWork.Setup(repo => repo.BarBeerRepository.Exists(It.IsAny<int>())).Returns(false);
            unitOfWork.Setup(repo => repo.BarRepository.Exists(It.IsAny<Expression<Func<Bar, bool>>>())).Returns(true);
            unitOfWork.Setup(repo => repo.BeerRepository.Exists(It.IsAny<Expression<Func<Beer, bool>>>())).Returns(true);
            unitOfWork.Setup(repo => repo.BarBeerRepository.Add(It.IsAny<BarBeer>())).ReturnsAsync(barbeer);

            /// Act
            var result = await barBeerService.AddBarBeer(barBeerDtoMock);

            //Assert
            Assert.Equal(barBeerDtoMock.BarId, result.BarId);
            Assert.Equal(barBeerDtoMock.BeerId, result.BeerId);
        }
    }
}
