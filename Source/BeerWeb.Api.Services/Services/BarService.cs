using AutoMapper;
using BeerWeb.Api.DataAccess.Interface;
using BeerWeb.Api.DataAccess.Model;
using BeerWeb.Api.Dto;
using BeerWeb.Api.Services.Interface;
using Serilog;

namespace BeerWeb.Api.Services
{
    public class BarService : IBarService
    {
        private readonly ILogger log;
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;

        public BarService(IUnitOfWork unitOfWork, ILogger logger, IMapper autoMapper)
        {
            this.unitOfWork = unitOfWork;
            log = logger;
            mapper = autoMapper;
        }

        /// <summary>
        /// Add New BAR
        /// </summary>
        /// <param name="t">BarDto</param>
        /// <returns></returns>
        public async Task<BarDto> AddBar(BarDto barDto)
        {
            ValidateAddRequest(barDto);
            var bar = mapper.Map<Bar>(barDto);
            var result = await unitOfWork.BarRepository.Add(bar);
            await unitOfWork.SaveAsync();
            return mapper.Map<BarDto>(result);
        }

        /// <summary>
        /// Update existing bar
        /// </summary>
        /// <param name="id"> Bar id to update</param>
        /// <param name="t">Bar with updated values</param>
        /// <returns></returns>
        public async Task UpdateBar(int id, BarDto barDto)
        {
            ValidateUpdateRequest(barDto);
            var bar = mapper.Map<Bar>(barDto);
            unitOfWork.BarRepository.Update(id, bar);
            await unitOfWork.SaveAsync();
        }

        /// <summary>
        /// Get all Bar
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<BarDto>> GetAll()
        {
            var result = await unitOfWork.BarRepository.GetAll();
            return result.Select(bar => mapper.Map<BarDto>(bar));
        }

        /// <summary>
        /// Get bar by barId
        /// </summary>
        /// <param name="id">barId</param>
        /// <returns></returns>
        public async Task<BarDto> GetById(int barId)
        {
            var result = await unitOfWork.BarRepository.GetById(barId);

            return mapper.Map<BarDto>(result);
        }

        /// <summary>
        /// Validate add request
        /// </summary>
        /// <param name="barDto"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private bool ValidateAddRequest(BarDto barDto)
        {
            if (unitOfWork.BarRepository.Exists(bar => bar.BarId == barDto.BarId))
            {
                var errorMsg = "Bar is already present.";
                log.Error(errorMsg);
                throw new ArgumentException(errorMsg);
            }

            return true;
        }

        /// <summary>
        /// Validate update request
        /// </summary>
        /// <param name="barDto"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private bool ValidateUpdateRequest(BarDto barDto)
        {
            if (!unitOfWork.BarRepository.Exists(bar => bar.BarId == barDto.BarId))
            {
                var errorMsg = $"Bar is not present for given bar id : {barDto.BarId}.";
                log.Error(errorMsg);
                throw new ArgumentException(errorMsg);
            }

            return true;
        }
    }
}
