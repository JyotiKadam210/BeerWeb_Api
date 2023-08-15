using AutoMapper;
using BeerWeb.Api.DataAccess.Interface;
using BeerWeb.Api.DataAccess.Model;
using BeerWeb.Api.Dto;
using BeerWeb.Api.Services.Interface;
using Serilog;

namespace BeerWeb.Api.Services
{
    public class BreweryBeersService : IBreweryBeersService
    {
        private readonly ILogger log;
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;

        public BreweryBeersService(IUnitOfWork unitOfWork, ILogger logger, IMapper autoMapper)
        {
            this.unitOfWork = unitOfWork;
            log = logger;
            mapper = autoMapper;
        }

        /// <summary>
        /// Add new Brewery Beer link
        /// </summary>
        /// <param name="breweryBeer">BreweryId, BeerId</param>
        /// <returns></returns>
        public async Task<BreweryBeerDto> AddBreweryBeer(BreweryBeerDto breweryBeerDto)
        {
            ValidateRequest(breweryBeerDto);

            var result = await unitOfWork.BreweryBeerRepository.Add(mapper.Map<BreweryBeer>(breweryBeerDto));
            await unitOfWork.SaveAsync();
            return mapper.Map<BreweryBeerDto>(result);
        }

        /// <summary>
        /// Get all Breweries with its associated Beers
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<BreweryBeersDto>> GetAllBreweriesWithBeers()
        {
            return await unitOfWork.BreweryBeerRepository.GetAll();
        }

        /// <summary>
        /// Get Brewery by its Id with all associated beers
        /// </summary>
        /// <param name="id">BreweryId</param>
        /// <returns></returns>
        public async Task<IEnumerable<BreweryBeersDto>> GetBrewerybyIdWithAllBeers(int id)
        {
            return await unitOfWork.BreweryBeerRepository.GetById(id);
        }

        /// <summary>
        /// Validate brewery beer link
        /// </summary>
        /// <param name="breweryBeerDto"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private bool ValidateRequest(BreweryBeerDto breweryBeerDto)
        {
            if (unitOfWork.BreweryBeerRepository.Exists(breweryBeerDto.Id))
            {
                var errorMsg = "BarBeer id is already present";
                log.Error(errorMsg);
                throw new ArgumentException(errorMsg);
            }

            if (!unitOfWork.BreweryRepository.Exists(brewery => brewery.BreweryId == breweryBeerDto.BreweryId))
            {
                var errorMsg = $"Brewery is not present with this Brewery : {breweryBeerDto.BreweryId}";
                log.Error(errorMsg);
                throw new ArgumentException(errorMsg);
            }

            if (!unitOfWork.BeerRepository.Exists(beer => beer.BeerId == breweryBeerDto.BeerId))
            {
                var errorMsg = $"Beer is not present with this BeerId : {breweryBeerDto.BeerId}";
                log.Error(errorMsg);
                throw new ArgumentException(errorMsg);
            }

            return true;
        }

    }
}
