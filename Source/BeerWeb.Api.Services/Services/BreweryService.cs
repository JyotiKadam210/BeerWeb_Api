using AutoMapper;
using BeerWeb.Api.DataAccess.Interface;
using BeerWeb.Api.DataAccess.Model;
using BeerWeb.Api.Dto;
using BeerWeb.Api.Services.Interface;
using Serilog;

namespace BeerWeb.Api.Services
{
    public class BreweryService : IBreweryService
    {
        private readonly ILogger log;
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;

        public BreweryService(IUnitOfWork unitOfWork, ILogger logger, IMapper autoMapper)
        {
            this.unitOfWork = unitOfWork;
            log = logger;
            mapper = autoMapper;
        }

        /// <summary>
        /// Add new Brewery
        /// </summary>
        /// <param name="t">Brewery</param>
        /// <returns></returns>
        public async Task<BreweryDto> AddBrewery(BreweryDto breweryDto)
        {
            ValidateAddRequest(breweryDto);
            var brewery = mapper.Map<Brewery>(breweryDto);
            var result = await unitOfWork.BreweryRepository.Add(brewery);
            await unitOfWork.SaveAsync();

            return mapper.Map<BreweryDto>(result);
        }

        /// <summary>
        /// Get all Breweries
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<BreweryDto>> GetAll()
        {
            var result = await unitOfWork.BreweryRepository.GetAll();
            return result.Select(brewery => mapper.Map<BreweryDto>(brewery));
        }

        /// <summary>
        /// Get Brewery by Id
        /// </summary>
        /// <param name="id">BreweryId</param>
        /// <returns></returns>
        public async Task<BreweryDto> GetById(int id)
        {
            var result = await unitOfWork.BreweryRepository.GetById(id);

            return mapper.Map<BreweryDto>(result);
        }

        /// <summary>
        /// Update Brewery
        /// </summary>
        /// <param name="id">BreweryId</param>
        /// <param name="t">Brewery</param>
        /// <returns></returns>
        public async Task UpdateBrewery(int id, BreweryDto breweryDto)
        {
            ValidateUpdateRequest(breweryDto);
            var brewery = mapper.Map<Brewery>(breweryDto);
            unitOfWork.BreweryRepository.Update(id, brewery);
            await unitOfWork.SaveAsync();
        }

        /// <summary>
        /// Validate add request
        /// </summary>
        /// <param name="breweryDto"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private bool ValidateAddRequest(BreweryDto breweryDto)
        {
            if (unitOfWork.BreweryRepository.Exists(brewery => brewery.BreweryId == breweryDto.BreweryId))
            {
                var errorMsg = $"Brewery is already present for given brewery id : {breweryDto.BreweryId}.";
                log.Error(errorMsg);
                throw new ArgumentException(errorMsg);
            }

            return true;
        }

        /// <summary>        
        /// Validate update request
        /// </summary>
        /// <param name="breweryDto"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private bool ValidateUpdateRequest(BreweryDto breweryDto)
        {
            if (!unitOfWork.BreweryRepository.Exists(brewery => brewery.BreweryId == breweryDto.BreweryId))
            {
                var errorMsg = $"Brewery is not present for given brewery id : {breweryDto.BreweryId}.";
                log.Error(errorMsg);
                throw new ArgumentException(errorMsg);
            }

            return true;
        }
    }
}
