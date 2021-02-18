using AutoMapper;
using HotelListing.Data;
using HotelListing.IRepository;
using HotelListing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly ILogger<HotelController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public HotelController(ILogger<HotelController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHotels()
        {
            try
            {
                var hotels = await _unitOfWork.Hotels.GetAll();
                var results = _mapper.Map<IList<HotelDTO>>(hotels);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong In The {nameof(GetHotels)}");
                return StatusCode(500, "Internal Server Error.Please Try Again Later");
            }

        }


        [HttpGet("{id:int}", Name = "GetHotel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetHotel(int id)
        {
            try
            {
                var hotel = await _unitOfWork.Hotels.Get(op => op.Id == id, new List<string> { "Country" });
                var result = _mapper.Map<HotelDTO>(hotel);
                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong In The {nameof(GetHotel)}");
                return StatusCode(500, "Internal Server Error.Please Try Again Later");
            }

        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> CreateHotel([FromBody] CreateHotelDTO hotelDTO)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Invalid Post Request In {nameof(CreateHotel)}");
                return BadRequest(ModelState);
            }
            try
            {
                var hotel = _mapper.Map<Hotel>(hotelDTO);
                await _unitOfWork.Hotels.Insert(hotel);
                await _unitOfWork.Save();  //hotel will be given an aoutomatic Id and get saved

                return CreatedAtRoute("GetHotel", new { id = hotel.Id }, hotel);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong In The {nameof(CreateHotel)}");
                return StatusCode(500, "Internal Server Error.Please Try Again Later");
            }
        }

        [Authorize]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> UpdateHotel(int id, [FromBody] UpdateHotelDTO hotelDTO)
        {
            if (!ModelState.IsValid || id < 1)
            {
                _logger.LogError($"Invalid UPDATE attempt In {nameof(UpdateHotel)}");
                return BadRequest();
            }
            try
            {
                var hotel = await _unitOfWork.Hotels.Get(q => q.Id == id); //get a Hotel with an id that is equivalent to the id that come throught as parameter
                if (hotel == null)
                {
                    _logger.LogError($"Invalid UPDATE attempt In {nameof(UpdateHotel)}");
                    return BadRequest("Submitted data is invalid");
                }

                _mapper.Map(hotelDTO, hotel);
                _unitOfWork.Hotels.Update(hotel);
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong In The {nameof(UpdateHotel)}");
                return StatusCode(500, "Internal Server Error.Please Try Again Later");
            }
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> DeleteHotel(int id)
        {
            if( id < 1)
            {
                _logger.LogError($"Invalid DELETE Attempt In {nameof(DeleteHotel)}");
                return BadRequest(ModelState);
            }

            try
            {
                var hotel = await _unitOfWork.Hotels.Get(q => q.Id == id);

                if (hotel == null)
                {
                    _logger.LogError($"Invalid DELETE attempt In {nameof(DeleteHotel)}");
                    return BadRequest("Submitted data is invalid");
                }

                await _unitOfWork.Hotels.Delete(id);
                await _unitOfWork.Save();
          

                return NoContent();

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong In The {nameof(DeleteHotel)}");
                return StatusCode(500, "Internal Server Error.Please Try Again Later");
            }
        }



    }
}
