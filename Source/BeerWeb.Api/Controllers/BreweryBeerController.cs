using BeerWeb.Api.Dto;
using BeerWeb.Api.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace BeerWeb.Api.Controllers
{
    [Route("api/brewery")]
    [ApiController]
    public class BreweryBeerController : ControllerBase
    {
        private readonly IBreweryBeersService breweryBeersService;

        /// <summary>
        /// Constructor for brewery beer
        /// </summary>
        /// <param name="breweryBeersService"></param>
        public BreweryBeerController(IBreweryBeersService breweryBeersService)
        {
            this.breweryBeersService = breweryBeersService;
        }

        /// <summary>
        /// Get all breweries with associated beers 
        /// </summary>
        /// <returns></returns>
        [HttpGet("beer")]
        public async Task<ActionResult<IEnumerable<BreweryBeersDto>>> GetBreweryBeers()
        {
           var breweries = await breweryBeersService.GetAllBreweriesWithBeers(); 

            if (breweries == null || !breweries.Any())
            {
                Log.Information("Brewery data not found.");
                return NotFound();
            }
            return Ok(breweries);
        }

        /// <summary>
        /// Get a single brewery by Id with associated beers 
        /// </summary>
        /// <param name="breweryId"></param>
        /// <returns></returns>
        [HttpGet("{breweryId}/beer")]
        public async Task<ActionResult<IEnumerable<BreweryBeersDto>>> GetBreweryBeers(int breweryId)
        {
            var result = await breweryBeersService.GetBrewerybyIdWithAllBeers(breweryId);
            if (result == null || !result.Any())
            {
                Log.Information($"Brewery is not found for BreweryId : {breweryId}");
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Add new Brewery Beer link
        /// </summary>
        /// <param name="breweryBeer"></param>
        /// <returns></returns>
        [HttpPost("beer")]
        public async Task<ActionResult<BreweryBeerDto>> PostBreweryBeer(BreweryBeerDto breweryBeer)
        {
            if (breweryBeer.Id <= 0)
            {
                return BadRequest();
            }
            await breweryBeersService.AddBreweryBeer(breweryBeer);

            return CreatedAtAction("GetBreweryBeers", new { id = breweryBeer.Id }, breweryBeer);
        }       
    }
}