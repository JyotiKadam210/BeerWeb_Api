using BeerWeb.Api.Dto;
using BeerWeb.Api.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace BeerWeb.Api.Controllers
{
    [Route("api/beer")]
    [ApiController]
    public class BeerController : ControllerBase
    {
        private readonly IBeerService beerService;

        /// <summary>
        /// Constructor for beer controller
        /// </summary>
        /// <param name="beerService"></param>
        public BeerController(IBeerService beerService)
        {
            this.beerService = beerService;
        }

        /// <summary>
        /// Get all beers with optional filtering query parameters for alcohol content (gtAlcoholByVolume = greater than, ltAlcoholByVolume = less than)
        /// </summary>
        /// <param name="gtAlcoholByVolume"></param>
        /// <param name="ltAlcoholByVolume"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BarDto>>> GetBeers([FromQuery] double gtAlcoholByVolume, [FromQuery] double ltAlcoholByVolume)
        {
            if (gtAlcoholByVolume < 0 || ltAlcoholByVolume < 0)
            {
                Log.Warning($"Invalid input tAlcoholByVolume (greater than) :{gtAlcoholByVolume}, ltAlcoholByVolume less than : {ltAlcoholByVolume}");
                return Problem("Invalid Input for tAlcoholByVolume or gtAlcoholByVolume.");
            }

            var result = await beerService.GetBeer(gtAlcoholByVolume, ltAlcoholByVolume);

            if (result == null || !result.Any())
            {
                Log.Information("Beer data not found.");
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Get beer deatils for given beerId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<BeerDto>> GetBeer(int id)
        {
            var beer = await beerService.GetById(id);

            if (beer == null)
            {
                Log.Information($"Beer is not found for BeerId : {id}");
                return NotFound();
            }

            return Ok(beer);
        }

        /// <summary>
        ///  Update beer deatils for given beer id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="beer"></param>
        /// <returns></returns>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateBeer(int id, BeerDto beer)
        {
            if (beer == null || id != beer.BeerId)
            {
                return BadRequest();
            }
            await beerService.UpdateBeer(id, beer);

            return Ok();
        }

        /// <summary>
        /// Add new beer deatils to beer table
        /// </summary>
        /// <param name="beer"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<BeerDto>> AddBeer(BeerDto beer)
        {
            if (beer == null || beer.BeerId <= 0)
            {
                return BadRequest();
            }

            await beerService.AddBeer(beer);
            return CreatedAtAction("GetBeer", new { id = beer.BeerId }, beer);
        }
    }
}