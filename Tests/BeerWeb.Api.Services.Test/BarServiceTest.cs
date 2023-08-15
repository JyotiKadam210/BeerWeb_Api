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
    public class BarServiceTest
    {
        private readonly IFixture fixture;
        protected readonly BeerStoreDbContext dbContext;
        private readonly ILogger log;
        private readonly IMapper mapper;
        private readonly IBarService barService;
        private readonly IUnitOfWork unitOfWork;

        public BarServiceTest()
        {
            fixture = new Fixture();
            var options = new DbContextOptionsBuilder<BeerStoreDbContext>()
           .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            dbContext = new BeerStoreDbContext(options);
            log = new LoggerConfiguration().MinimumLevel.Information().WriteTo.Console().CreateLogger();

            mapper = new Mapper(new MapperConfiguration(x =>
            {

                x.CreateMap<Bar, BarDto>().ReverseMap();

            }));
            
            unitOfWork = new UnitOfWork(dbContext);
            barService = new BarService(unitOfWork, log, mapper);

            dbContext.Database.EnsureCreated();
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllBars()
        {
            /// Arrange
            var mockBar = fixture.CreateMany<Bar>(5).ToList();
            dbContext.Bars.AddRange(mockBar);
            await dbContext.SaveChangesAsync();

            /// Act
            var result = await barService.GetAll();

            /// Assert                       
            Assert.NotNull(result);
            Assert.Equal(mockBar.Count, result?.Count());
        }

        [Theory]
        [InlineData(500)]
        public async Task GetById_ShouldReturnBar_WithGivenId(int id)
        {
            /// Arrange
            fixture.Register(() => id);
            int mockId = fixture.Create<int>();

            var mockBar = fixture.Build<Bar>()
                .With(x => x.BarId, mockId)
                .Create();

            dbContext.Bars.Add(mockBar);
            await dbContext.SaveChangesAsync();

            /// Act
            var result = await barService.GetById(id);

            /// Assert                       
            Assert.NotNull(result);
            Assert.Equal(mockBar.BarId, result.BarId);
        }

        [Fact]
        public async Task AddBar_ShouldAddNewBar()
        {
            /// Arrange
            var mockBarDto = fixture.Build<BarDto>().Create();

            /// Act
            var result = await barService.AddBar(mockBarDto);

            /// Assert  
            Assert.Equal(mockBarDto.BarId, result.BarId);
        }

        [Fact]
        public async Task UpdateBar_ShouldReturnError_WhenBarIsNotPresent()
        {
            /// Arrange
            fixture.Register(() => 100);
            int mockId = fixture.Create<int>();

            var mockBar = fixture.Build<BarDto>()
                .With(x => x.BarId, mockId)
                .Create();

            /// Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => barService.UpdateBar(mockId, mockBar));

            /// Assert
            Assert.Equal("Bar is not present for given bar id : 100.", ex.Message);           
        }
    }
}