using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        /// <summary>
        /// Returns the list of all villages from LocationData
        /// </summary>
        [HttpGet]
        public IActionResult GetVillages()
        {
            var villages = LocationData.Coordinates.Keys
                .OrderBy(x => x)
                .ToList();

            return Ok(villages);
        }
    }
}
