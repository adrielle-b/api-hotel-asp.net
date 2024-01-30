using Microsoft.AspNetCore.Mvc;
using TrybeHotel.Models;
using TrybeHotel.Repository;
using TrybeHotel.Dto;
using TrybeHotel.Services;


namespace TrybeHotel.Controllers
{
    [ApiController]
    [Route("geo")]
    public class GeoController : Controller
    {
        private readonly IHotelRepository _repository;
        private readonly IGeoService _geoService;


        public GeoController(IHotelRepository repository, IGeoService geoService)
        {
            _repository = repository;
            _geoService = geoService;
       
        }

        [HttpGet]
        [Route("status")]
        public async Task<IActionResult> GetStatus()
        {
            var resultStatus = await _geoService.GetGeoStatus();
            if (resultStatus == null)
            {
                return BadRequest();
            }
            return Ok(resultStatus);
        } 

        [HttpGet]
        [Route("address")]
        public async Task<IActionResult> GetHotelsByLocation([FromBody] GeoDto address)
        {
            var resultDistance = await _geoService.GetHotelsByGeo(address, _repository);
            if (resultDistance == null)
            {
                return BadRequest();
            }
            return Ok(resultDistance);
        }
    }
}