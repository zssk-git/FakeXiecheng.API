using AutoMapper;
using FakeXiecheng.API.Dtos;
using FakeXiecheng.API.Moldes;
using FakeXiecheng.API.ResourceParameters;
using FakeXiecheng.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FakeXiecheng.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TouristRoutesController : ControllerBase
    {
        private readonly ITouristRouteRepository _touristRouteRepository;
        private readonly IMapper _mapper;

        public TouristRoutesController(ITouristRouteRepository touristRouteRepository,IMapper mapper)
        {
            _touristRouteRepository = touristRouteRepository;
            _mapper = mapper;
        }
        ///// <summary>
        ///// api/touristRoutes?keyword=传入的参数&rating=largerThan2
        ///// http://localhost:56036/api/touristroutes?keyword=埃及&rating=largerThan2
        ///// http://localhost:56036/api/touristroutes?rating=largerThan4
        ///// </summary>
        ///// <param name="keyword">FromQuery vs FromBody</param>
        ///// <param name="rating">小于lessThan,大于largerThan,等于equalTo lessThan3,largerThan2,equalTo5</param>
        ///// <returns></returns>
        //[HttpGet]
        //[HttpHead]
        //public IActionResult GetTouristRoutes([FromQuery] string keyword,string rating)
        //{
        //    Regex regex = new Regex(@"([A-Za-z0-9\-]+)(\d+)");
        //    string operatorType="";
        //    int raringValue= -1;

        //    Match match = regex.Match(rating);
        //    if (match.Success)
        //    {
        //        operatorType = match.Groups[1].Value;
        //        raringValue = Int32.Parse(match.Groups[2].Value);
        //    }
        //    var touristRoutesFromRepo  = _touristRouteRepository.GetTouristRoutes(keyword, operatorType,raringValue);
        //    if (touristRoutesFromRepo == null || touristRoutesFromRepo.Count() <= 0)
        //    {
        //        return NotFound("没有旅游路线");
        //    }
        //    var touristRoutesDto = _mapper.Map<IEnumerable<TouristRouteDto>>(touristRoutesFromRepo);
        //    return Ok(touristRoutesDto);
        //}



        /// <summary>
        ///  获取所有TouristRoutes
        ///  api/touristRoutes?keyword=传入的参数&rating=largerThan2
        ///  http://localhost:5000/api/TouristRoutes?Keyword=埃及&RatingOperator=largerThan&RatingValue=3
        ///  http://localhost:5000/api/TouristRoutes?Keyword=埃及&Rating=largerThan4
        ///  http://localhost:5000/api/TouristRoutes?Rating=largerThan4
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetTouristRoutes([FromQuery] TouristRouteResourceParameters parameters)
        {
            
            var touristRoutesFromRepo = _touristRouteRepository.GetTouristRoutes(parameters.Keyword, parameters.RatingOperator, parameters.RatingValue);
            if (touristRoutesFromRepo == null || touristRoutesFromRepo.Count() <= 0)
            {
                return NotFound("没有旅游路线");
            }
            var touristRoutesDto = _mapper.Map<IEnumerable<TouristRouteDto>>(touristRoutesFromRepo);
            return Ok(touristRoutesDto);
        }
        /// <summary>
        /// 通过Id获取TouristRoute
        /// api/touristRoutes/{touristRouteId}
        /// http://localhost:5000/api/TouristRoutes/39996f34-013c-4fc6-b1b3-0c1036c47112
        /// </summary>
        /// <param name="touristRouteId"></param>
        /// <returns></returns>
        [HttpGet("{touristRouteId}", Name = "GetTouristRouteById")]
        public IActionResult GetTouristRouteById(Guid touristRouteId)
        {
            var touristRouteFromRepo = _touristRouteRepository.GetTouristRoute(touristRouteId);
            if (touristRouteFromRepo == null)
            {
                return NotFound($"旅游路线{touristRouteId} 找不到");
            }
            //var touristRouteDto = new TouristRouteDto()
            //{
            //    Id = touristRouteFromRepo.Id,
            //    Title = touristRouteFromRepo.Title,
            //    Description = touristRouteFromRepo.Description,
            //    Price = touristRouteFromRepo.OriginalPrice * (decimal)(touristRouteFromRepo.DiscountPresent ?? 1),
            //    CreateTime = touristRouteFromRepo.CreateTime,
            //    UpdateTime = touristRouteFromRepo.UpdateTime,
            //    Features = touristRouteFromRepo.Features,
            //    Fees = touristRouteFromRepo.Fees,
            //    Notes = touristRouteFromRepo.Notes,
            //    Rating = touristRouteFromRepo.Rating,
            //    TravelDays = touristRouteFromRepo.TravelDays.ToString(),
            //    TripType = touristRouteFromRepo.TripType.ToString(),
            //    DepartureCity = touristRouteFromRepo.DepartureCity.ToString()

            //};
            var touristRouteDto = _mapper.Map<TouristRouteDto>(touristRouteFromRepo);

            return Ok(touristRouteDto);
        }
        /// <summary>
        /// 添加TouristRoute
        /// </summary>
        /// <param name="touristRouteForCreationDto"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CreateTouristRoute([FromBody] TouristRouteForCreationDto touristRouteForCreationDto)
        {
            var touristRouteModel = _mapper.Map<TouristRoute>(touristRouteForCreationDto);
            _touristRouteRepository.AddTouristRoute(touristRouteModel);
            _touristRouteRepository.Save();
            var touristRouteToReture = _mapper.Map<TouristRouteDto>(touristRouteModel);
            return CreatedAtRoute("GetTouristRouteById", new { touristRouteId = touristRouteToReture.Id }, touristRouteToReture);
        }
        /// <summary>
        /// 更新TouristRoute
        /// </summary>
        /// <param name="touristRouteId"></param>
        /// <param name="touristRouteForUpdateDto"></param>
        /// <returns></returns>
        [HttpPut("{touristRouteId}")]
        public IActionResult UpdateTouristRoute([FromRoute] Guid touristRouteId,[FromBody] TouristRouteForUpdateDto touristRouteForUpdateDto)
        {
            if (!_touristRouteRepository.TouristRouteExists(touristRouteId))
            {
                return NotFound("旅游路线找不到");
            }
            var touristRouteFormRepo = _touristRouteRepository.GetTouristRoute(touristRouteId);
            //1.映射dto
            //2.更新dto
            //3.映射model
            _mapper.Map(touristRouteForUpdateDto, touristRouteFormRepo);
            _touristRouteRepository.Save();
            return NoContent();

        }
    }
}
