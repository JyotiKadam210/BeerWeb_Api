using BeerWeb.Api.Dto;
using BeerWeb.Api.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace BeerWeb.Api.Controllers
{
    [Route("api/bar")]
    [ApiController]
    public class BarController : ControllerBase
    {
        private readonly IBarService barService;

        /// <summary>
        /// Constructor for bar controller
        /// </summary>
        /// <param name="barService"></param>
        public BarController(IBarService barService)
        {
            this.barService = barService;
        }

        /// <summary>
        /// Get details for all bars
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BarDto>>> GetBars()
        {
            var bars = await barService.GetAll();
            if (bars == null || !bars.Any())
            {
                Log.Information("Bar data not found.");
                return NotFound();
            }
            return Ok(bars);
        }

        /// <summary>
        /// Get bar details for given barId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<BarDto>> GetBar(int id)
        {
            var bar = await barService.GetById(id);

            if (bar == null)
            {
                Log.Information($"Bar is not found for BarId : {id}");
                return NotFound();
            }

            return Ok(bar);
        }

        /// <summary>
        /// Update bar deatils for given barId
        /// </summary>
        /// <param name="id"> BarId</param>
        /// <param name="bar">bar details</param>
        /// <returns></returns>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateBar(int id, BarDto bar)
        {
            if (bar == null || id != bar.BarId)
            {
                return BadRequest();
            }

            await barService.UpdateBar(id, bar);
            return Ok();
        }

        /// <summary>
        /// Add new bar to bar table
        /// </summary>
        /// <param name="bar"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<BarDto>> AddBar(BarDto bar)
        {
            if (bar == null || bar.BarId <= 0)
            {
                return BadRequest();
            }
            await barService.AddBar(bar);

            return CreatedAtAction("GetBar", new { id = bar.BarId }, bar);
        }
    }
}