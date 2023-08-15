using BeerWeb.Api.Dto;
using BeerWeb.Api.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace BeerWeb.Api.Controllers
{
    [Route("api/brewery")]
    [ApiController]
    public class BreweryController : ControllerBase
    {

        private readonly IBreweryService breweryService;

        /// <summary>
        /// Constructor for brewery controller
        /// </summary>
        /// <param name="breweryService"></param>
        public BreweryController(IBreweryService breweryService)
        {
            this.breweryService = breweryService;
        }

        /// <summary>
        /// Get all Breweries details 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BreweryDto>>> GetBreweries()
        {
            var breweries = await breweryService.GetAll();

            if (breweries == null || !breweries.Any())
            {
                Log.Information($"Brewery data not found.");
                return NotFound();
            }
            return Ok(breweries);
        }

        /// <summary>
        /// Get brewery details by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<BreweryDto>> GetBrewery(int id)
        {
            var brewery = await breweryService.GetById(id);

            if (brewery == null)
            {
                Log.Information($"Brewery is not found for BreweryId : {id}");
                return NotFound();
            }

            return Ok(brewery);
        }

        /// <summary>
        /// Update Brewery details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="brewery"></param>
        /// <returns></returns>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateBrewery(int id, BreweryDto brewery)
        {
            if (id != brewery.BreweryId)
            {
                return BadRequest();
            }
            await breweryService.UpdateBrewery(id, brewery);
            return Ok();
        }

        /// <summary> 
        /// Add Brewery details in the Brewery Table
        /// </summary>
        /// <param name="brewery"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<BreweryDto>> AddBrewery(BreweryDto brewery)
        {
            if (brewery == null || brewery.BreweryId <= 0)
            {
                return BadRequest();
            }
            await breweryService.AddBrewery(brewery);
            return CreatedAtAction("GetBrewery", new { id = brewery.BreweryId }, brewery);
        }
    }
}