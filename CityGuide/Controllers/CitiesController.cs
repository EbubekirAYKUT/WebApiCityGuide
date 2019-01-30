using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CityGuide.Data;
using CityGuide.Dtos;
using CityGuide.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CityGuide.Controllers
{
    [Produces("application/json")]
    [Route("api/Cities")]
    public class CitiesController : Controller
    {
        IAppRepository _appRepository;
        private IMapper _mapper;
        public CitiesController(IAppRepository appRepository,IMapper mapper)
        {
            _appRepository = appRepository;
            _mapper = mapper;
        }

        public IActionResult GetCities()
        {
            var cities = _appRepository.GetCities();
            var citiesToReturn = _mapper.Map<List<CityForListDto>>(cities);
            return Ok(citiesToReturn);
        }

        [HttpPost]
        [Route("add")]
        public IActionResult Add([FromBody]City city)
        {
            _appRepository.Add(city);
            _appRepository.SaveAll();
            return Ok(city);
        }

        [HttpGet]
        [Route("detail")]
        public IActionResult GetCitiesById(int id)
        {
            var city= _appRepository.GetCityById(id);
            var cityToReturn = _mapper.Map<CityForDetailDto>(city);
            return Ok(city);
        }

        [HttpGet]
        [Route("Photos")]
        public IActionResult GetPhotosByCity(int cityId)
        {
            var photos = _appRepository.GetPhotosByCity(cityId);           
            return Ok(photos);
        }
    }
}