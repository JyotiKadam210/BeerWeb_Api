
using AutoFixture;
using AutoMapper;
using BeerWeb.Api.DataAccess.DatabaseContext;
using BeerWeb.Api.DataAccess.Interface;
using BeerWeb.Api.DataAccess.Model;
using BeerWeb.Api.DataAccess.UnitOfWork;
using BeerWeb.Api.Dto;
using BeerWeb.Api.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace BeerWeb.Api.Services.Test
{
    public class BeerServiceTest
    {
        private readonly IFixture fixture;
        protected readonly BeerStoreDbContext dbContext;
        private readonly ILogger log;
        private readonly IMapper mapper;
        private readonly IBeerService beerService;
        private readonly IUnitOfWork unitOfWork;

        public BeerServiceTest()
        {
            fixture = new Fixture();
            var options = new DbContextOptionsBuilder<BeerStoreDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

            dbContext = new BeerStoreDbContext(options);

            mapper = new Mapper(new MapperConfiguration(cfg =>
            {

                cfg.CreateMap<Beer, BeerDto>().ReverseMap();

            }));

            unitOfWork = new UnitOfWork(dbContext);

            log = new LoggerConfiguration().MinimumLevel.Information().WriteTo.Console().CreateLogger();

            beerService = new BeerService(unitOfWork, log, mapper);

            dbContext.Database.EnsureCreated();
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllBeers()
        {
            /// Arrange
            var mockBeer = fixture.CreateMany<Beer>(5).ToList();
            dbContext.Beers.AddRange(mockBeer);
            await dbContext.SaveChangesAsync();

            /// Act
            var result = await beerService.GetAll();

            /// Assert                       
            Assert.NotNull(result);
            Assert.Equal(mockBeer.Count, result?.Count());
        }

        [Theory]
        [InlineData(500)]
        public async Task GetById_ShouldReturnBeer_WithGivenId(int id)
        {
            /// Arrange
            fixture.Register(() => id);
            int mockId = fixture.Create<int>();

            var mockBeer = fixture.Build<Beer>()
                .With(x => x.BeerId, mockId)
                .Create();

            dbContext.Beers.Add(mockBeer);
            await dbContext.SaveChangesAsync();

            /// Act
            var result = await beerService.GetById(id);

            /// Assert                       
            Assert.NotNull(result);
            Assert.Equal(mockBeer.BeerId, result.BeerId);
        }

        [Fact]
        public async Task AddBeer_ShouldAddNewBeer()
        {
            /// Arrange
            var mockBeerDto = fixture.Build<BeerDto>().Create();

            /// Act
            var result = await beerService.AddBeer(mockBeerDto);

            /// Assert  
            Assert.Equal(mockBeerDto.BeerId, result.BeerId);
        }

        [Fact]
        public async Task UpdateBeer_ShouldReturnError_WhenBeerIsNotPresent()
        {
            /// Arrange
            fixture.Register(() => 100);
            int mockId = fixture.Create<int>();

            var mockBeer = fixture.Build<BeerDto>()
                .With(x => x.BeerId, mockId)
                .Create();

            /// Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => beerService.UpdateBeer(mockId, mockBeer));

            /// Assert
            Assert.Equal("Beer is not present for given beer id : 100.", ex.Message);
        } 
    }
}