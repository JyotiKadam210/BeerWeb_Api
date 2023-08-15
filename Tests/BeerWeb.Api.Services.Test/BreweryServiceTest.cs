
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
    public class BreweryServiceTest
    {
        private readonly IFixture fixture;
        protected readonly BeerStoreDbContext dbContext;
        private readonly ILogger log;
        private readonly IMapper mapper;
        private readonly IBreweryService breweryServiceMock;
        private readonly IUnitOfWork unitOfWork;

        public BreweryServiceTest()
        {
            fixture = new Fixture();

            var options = new DbContextOptionsBuilder<BeerStoreDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

            dbContext = new BeerStoreDbContext(options);
            dbContext.Database.EnsureCreated();

            log = new LoggerConfiguration().MinimumLevel.Information().WriteTo.Console().CreateLogger();

            mapper = new Mapper(new MapperConfiguration(x =>
            {
                x.CreateMap<Brewery, BreweryDto>().ReverseMap();

            }));

            unitOfWork = new UnitOfWork(dbContext);

            breweryServiceMock = new BreweryService(unitOfWork, log, mapper);

            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllBreweries()
        {
            /// Arrange
            var mockBrewery = fixture.CreateMany<Brewery>(5).ToList();
            dbContext.Breweries.AddRange(mockBrewery);
            await dbContext.SaveChangesAsync();

            /// Act
            var result = await breweryServiceMock.GetAll();

            /// Assert                       
            Assert.NotNull(result);
            Assert.Equal(mockBrewery.Count, result?.Count());
        }

        [Theory]
        [InlineData(500)]
        public async Task GetById_ShouldReturnBrewery_WithGivenId(int id)
        {
            /// Arrange
            fixture.Register(() => id);
            int mockId = fixture.Create<int>();

            var mockBrewery = fixture.Build<Brewery>()
                .With(x => x.BreweryId, mockId)
                .Create();

            dbContext.Breweries.Add(mockBrewery);
            await dbContext.SaveChangesAsync();

            /// Act
            var result = await breweryServiceMock.GetById(id);

            /// Assert                       
            Assert.NotNull(result);
            Assert.Equal(mockBrewery.BreweryId, result.BreweryId);
        }

        [Fact]
        public async Task AddBrewery_ShouldAddNewBrewery()
        {
            /// Arrange
            var mockBreweryDto = fixture.Build<BreweryDto>().Create();

            /// Act
            var result = await breweryServiceMock.AddBrewery(mockBreweryDto);

            /// Assert  
            Assert.Equal(mockBreweryDto.BreweryId, result.BreweryId);
        }

        [Fact]
        public async Task UpdateBrewery_ShouldReturnError_WhenBreweryIsNotPresent()
        {
            /// Arrange
            fixture.Register(() => 100);
            int mockId = fixture.Create<int>();

            var mockBrewery = fixture.Build<BreweryDto>()
                .With(x => x.BreweryId, mockId)
                .Create();

            /// Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => breweryServiceMock.UpdateBrewery(mockId, mockBrewery));

            /// Assert
            Assert.Equal("Brewery is not present for given brewery id : 100.", ex.Message);
        }
    }
}