using BeerWeb.Api.Dto;
using BeerWeb.Api.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace BeerWeb.Api.Controllers
{
    [ApiController]
    public class BarBeerController : ControllerBase
    {
        private readonly IBarBeersService barBeersService;

        /// <summary>
        /// Constructor for bar beer controller 
        /// </summary>
        /// <param name="barBeersService">BarBeerService</param>
        public BarBeerController(IBarBeersService barBeersService)
        {
            this.barBeersService = barBeersService;
        }

        /// <summary>
        /// Get all bar with its associated beers
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/bar/beer")]
        public async Task<ActionResult<IEnumerable<BarBeersDto>>> GetBarBeers()
        {           
            var bars = await barBeersService.GetAllBarsWithBeers();  
            
            if (bars == null || !bars.Any())
            {
                Log.Information("Bar data not found.");
                return NotFound();
            }

            return Ok(bars);
        }

        /// <summary>
        /// Get Bar by BarId with its associated beers
        /// </summary>
        /// <param name="barId"></param>
        /// <returns></returns>
        [HttpGet("api/bar/{barId:int}/beer")]
        public async Task<ActionResult<IEnumerable<BarBeersDto>>> GetBarBeers(int barId)
        {
            var result = await barBeersService.GetBarbyIdWithAllBeers(barId);

            if (result == null || !result.Any())
            {
                Log.Information($"Bar is not found for BarId : {barId}");
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Add bar beer link
        /// </summary>
        /// <param name="barBeer">bar and beer id</param>
        /// <returns></returns>
        [HttpPost("api/bar/beer")]
        public async Task<ActionResult<BarBeerDto>> PostBarBeer(BarBeerDto barBeer)
        {
            if (barBeer == null || barBeer.Id <= 0)
            {
                return BadRequest();
            }
            await barBeersService.AddBarBeer(barBeer);

            return CreatedAtAction("GetBarBeers", new { id = barBeer.Id }, barBeer);
        }
    }
}