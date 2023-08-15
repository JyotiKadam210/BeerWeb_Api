using AutoMapper;
using BeerWeb.Api.DataAccess.Interface;
using BeerWeb.Api.DataAccess.Model;
using BeerWeb.Api.Dto;
using BeerWeb.Api.Services.Interface;
using Serilog;

namespace BeerWeb.Api.Services
{
    public class BarBeerService : IBarBeersService    
    {  
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger log;

        public BarBeerService(IUnitOfWork unitOfWork, ILogger logger, IMapper autoMapper)
        {
            this.unitOfWork = unitOfWork;
            log = logger;
            mapper = autoMapper;    
        }

        /// <summary>
        /// Add new Bar Beer Link
        /// </summary>
        /// <param name="barBeer">BarId and BeerId</param>
        /// <returns></returns>
        public async Task<BarBeerDto> AddBarBeer(BarBeerDto barBeer)
        {
            ValidateRequest(barBeer);

            var barbeer = mapper.Map<BarBeer>(barBeer);
            var result = await unitOfWork.BarBeerRepository.Add(barbeer);
            await unitOfWork.SaveAsync();
            return mapper.Map<BarBeerDto>(result);
        }       

        /// <summary>
        /// Get all Bars with its associated beers
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<BarBeersDto>> GetAllBarsWithBeers()
        {
            return await unitOfWork.BarBeerRepository.GetAll();
        }

        /// <summary>
        /// Get Bar by BarId with its associated beers
        /// </summary>
        /// <param name="id">BarId</param>
        /// <returns></returns>
        public async Task<IEnumerable<BarBeersDto>> GetBarbyIdWithAllBeers(int id)
        {
            return await unitOfWork.BarBeerRepository.GetById(id);
        }

        /// <summary>
        /// Validate bar beer link
        /// </summary>
        /// <param name="barBeer"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private bool ValidateRequest(BarBeerDto barBeer)
        {
            if (unitOfWork.BarBeerRepository.Exists(barBeer.Id))
            {
                var errorMsg = $"BarBeer id is already present : {barBeer.Id}";
                log.Error(errorMsg);
                throw new ArgumentException(errorMsg);
            }

            if (!unitOfWork.BarRepository.Exists(bar => bar.BarId == barBeer.BarId))
            {
                var errorMsg = $"Bar is not present for this BarId : {barBeer.BarId}";
                log.Error(errorMsg);
                throw new ArgumentException(errorMsg);
            }

            if (!unitOfWork.BeerRepository.Exists(beer => beer.BeerId == barBeer.BeerId))
            {
                var errorMsg = $"Beer is not present for this BeerId : {barBeer.BeerId}";
                log.Error(errorMsg);
                throw new ArgumentException(errorMsg);
            }

            return true;
        }
    }
}
