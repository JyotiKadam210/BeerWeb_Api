using AutoMapper;
using BeerWeb.Api.DataAccess.Interface;
using BeerWeb.Api.DataAccess.Model;
using BeerWeb.Api.Dto;
using BeerWeb.Api.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace BeerWeb.Api.Services
{
    public class BeerService : IBeerService
    {
        private readonly ILogger log;
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;

        public BeerService(IUnitOfWork unitOfWork, ILogger logger, IMapper autoMapper)
        {
            this.unitOfWork = unitOfWork;
            log = logger;
            mapper = autoMapper;
        }

        /// <summary>
        /// Add new Beer
        /// </summary>
        /// <param name="t">Beer</param>
        /// <returns></returns>
        public async Task<BeerDto> AddBeer(BeerDto beerDto)
        {
            ValidateAddRequest(beerDto);
            var beer = mapper.Map<Beer>(beerDto);
            var result = await unitOfWork.BeerRepository.Add(beer);
            await unitOfWork.SaveAsync();
            return mapper.Map<BeerDto>(result);
        }

        /// <summary>
        /// Get all beers with optional filtering query parameters for alcohol content (gtAlcoholByVolume = greater than, ltAlcoholByVolume = less than)
        /// </summary>
        /// <param name="gtAlcoholByVolume">Value greater than</param>
        /// <param name="ltAlcoholByVolume">Value less than</param>
        /// <returns></returns>
        public async Task<IEnumerable<BeerDto>> GetBeer([FromQuery] double gtAlcoholByVolume, [FromQuery] double ltAlcoholByVolume)
        {
           var result =  await unitOfWork.BeerRepository.GetBeerByAlcoholParameter(gtAlcoholByVolume, ltAlcoholByVolume);

            return result.Select(beer => mapper.Map<BeerDto>(beer));

        }

        /// <summary>
        /// Update existing Beer
        /// </summary>
        /// <param name="id">BeerId to update</param>
        /// <param name="t">Beer object with updated values</param>
        /// <returns></returns>
        public async Task UpdateBeer(int id, BeerDto beerDto)
        {
            ValidateUpdateRequest(beerDto);
            var beer = mapper.Map<Beer>(beerDto);
            unitOfWork.BeerRepository.Update(id, beer);
            await unitOfWork.SaveAsync();
        }

        /// <summary>
        /// Get all Beer
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<BeerDto>> GetAll()
        {
            var result = await unitOfWork.BeerRepository.GetAll();
            return result.Select(beer => mapper.Map<BeerDto>(beer));
        }

        /// <summary>
        /// Get Beer with BeerId
        /// </summary>
        /// <param name="id">BeerId</param>
        /// <returns></returns>
        public async Task<BeerDto> GetById(int id)
        {
            var result = await unitOfWork.BeerRepository.GetById(id);

            return mapper.Map<BeerDto>(result);
        }

        /// <summary>
        /// Validate add request
        /// </summary>
        /// <param name="barDto"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private bool ValidateAddRequest(BeerDto beerDto)
        {
            if (unitOfWork.BeerRepository.Exists(beer => beer.BeerId == beerDto.BeerId))
            {
                var errorMsg = $"Beer is already present for given beer id : {beerDto.BeerId}.";
                log.Error(errorMsg);
                throw new ArgumentException(errorMsg);
            }

            return true;
        }

        /// <summary>        /// Validate update request
        /// </summary>
        /// <param name="barDto"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private bool ValidateUpdateRequest(BeerDto beerDto)
        {
            if (!unitOfWork.BeerRepository.Exists(beer => beer.BeerId == beerDto.BeerId))
            {
                var errorMsg = $"Beer is not present for given beer id : {beerDto.BeerId}.";
                log.Error(errorMsg);
                throw new ArgumentException(errorMsg);
            }

            return true;
        }
    }
}
