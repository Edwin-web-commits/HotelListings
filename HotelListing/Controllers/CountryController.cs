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
    public class CountryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CountryController> _logger;
        private readonly IMapper _mapper;

        public CountryController(IUnitOfWork unitOfWork, ILogger<CountryController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCountries([FromQuery] RequestParams requestParams) //[FromQuery] we will look inside of the query string that is being passed over for the Params(Parameters) request.So we are looking for params that match the names that are outlined in our request model
        {
            
                var countries = await _unitOfWork.Countries.GetPagedList(requestParams);
                var results =  _mapper.Map<IList<CountryDTO>>(countries);
                return Ok(results);
           
        }

        [HttpGet("{id:int}", Name = "GetCountry")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetCountry(int id)
        {
            
                var country = await _unitOfWork.Countries.Get(q => q.Id == id, new List<string> { "Hotels"}) ;
                var result = _mapper.Map<CountryDTO>(country);
                return Ok(result);
            
            
        }

        [Authorize(Roles="Administration")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> CreateCountry([FromBody] CreateCountryDTO countryDTO) //[FromBody] means the request has a data in the body and we retrieve that data
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Invalid Post Attempt In {nameof(CreateCountry)}");
                return BadRequest(ModelState);
            }

            
                var country = _mapper.Map<Country>(countryDTO);
                await _unitOfWork.Countries.Insert(country);
                await _unitOfWork.Save();

                return CreatedAtRoute("GetCountry", new { id = country.Id }, country);
           
        }

        [Authorize]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> UpdateCountry(int id, [FromBody] UpdateCountryDTO countryDTO)
        {
            if (!ModelState.IsValid || id < 1)
            {
                _logger.LogError($"Invalid UPDATE attempt In {nameof(UpdateCountry)}");
                return BadRequest(ModelState);
            }
           
                var country = await _unitOfWork.Countries.Get(q => q.Id == id); //get a Hotel with an id that is equivalent to the id that come throught as parameter
                if (country == null)
                {
                    _logger.LogError($"Invalid UPDATE attempt In {nameof(UpdateCountry)}");
                    return BadRequest("Submitted data is invalid");
                }

                _mapper.Map(countryDTO, country);
                _unitOfWork.Countries.Update(country);
                await _unitOfWork.Save();

                return NoContent();
            }

        [Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> DeleteCountry(int id)
        {
            if (id < 1)
            {
                _logger.LogError($"Invalid DELETE Attempt In {nameof(DeleteCountry)}");
                return BadRequest(ModelState);
            }


            var country = await _unitOfWork.Countries.Get(q => q.Id == id);

            if (country == null)
            {
                _logger.LogError($"Invalid DELETE attempt In {nameof(DeleteCountry)}");
                return BadRequest("Submitted data is invalid");
            }

            await _unitOfWork.Countries.Delete(id);
            await _unitOfWork.Save();


            return NoContent();


        }



    }
}
