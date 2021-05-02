using AutoMapper;
using FakeXiecheng.API.Dtos;
using FakeXiecheng.API.Helper;
using FakeXiecheng.API.Moldes;
using FakeXiecheng.API.ResourceParameters;
using FakeXiecheng.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        /**
        /// <summary>
        /// api/touristRoutes?keyword=传入的参数&rating=largerThan2
        /// http://localhost:56036/api/touristroutes?keyword=埃及&rating=largerThan2
        /// http://localhost:56036/api/touristroutes?rating=largerThan4
        /// </summary>
        /// <param name="keyword">FromQuery vs FromBody</param>
        /// <param name="rating">小于lessThan,大于largerThan,等于equalTo lessThan3,largerThan2,equalTo5</param>
        /// <returns></returns>
        [HttpGet]
        [HttpHead]
        public IActionResult GetTouristRoutes([FromQuery] string keyword, string rating)
        {
            Regex regex = new Regex(@"([A-Za-z0-9\-]+)(\d+)");
            string operatorType = "";
            int raringValue = -1;

            Match match = regex.Match(rating);
            if (match.Success)
            {
                operatorType = match.Groups[1].Value;
                raringValue = Int32.Parse(match.Groups[2].Value);
            }
            var touristRoutesFromRepo = _touristRouteRepository.GetTouristRoutes(keyword, operatorType, raringValue);
            if (touristRoutesFromRepo == null || touristRoutesFromRepo.Count() <= 0)
            {
                return NotFound("没有旅游路线");
            }
            var touristRoutesDto = _mapper.Map<IEnumerable<TouristRouteDto>>(touristRoutesFromRepo);
            return Ok(touristRoutesDto);
        }

        */

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
        public async Task<IActionResult> GetTouristRoutes([FromQuery] TouristRouteResourceParameters parameters)
        {
            
            var touristRoutesFromRepo = await _touristRouteRepository.GetTouristRoutesAsync(parameters.Keyword, parameters.RatingOperator, parameters.RatingValue);
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
        public async Task<IActionResult> GetTouristRouteById(Guid touristRouteId)
        {
            var touristRouteFromRepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            if (touristRouteFromRepo == null)
            {
                return NotFound($"旅游路线{touristRouteId} 找不到");
            }
            /**
            var touristRouteDto = new TouristRouteDto()
            {
                Id = touristRouteFromRepo.Id,
                Title = touristRouteFromRepo.Title,
                Description = touristRouteFromRepo.Description,
                Price = touristRouteFromRepo.OriginalPrice * (decimal)(touristRouteFromRepo.DiscountPresent ?? 1),
                CreateTime = touristRouteFromRepo.CreateTime,
                UpdateTime = touristRouteFromRepo.UpdateTime,
                Features = touristRouteFromRepo.Features,
                Fees = touristRouteFromRepo.Fees,
                Notes = touristRouteFromRepo.Notes,
                Rating = touristRouteFromRepo.Rating,
                TravelDays = touristRouteFromRepo.TravelDays.ToString(),
                TripType = touristRouteFromRepo.TripType.ToString(),
                DepartureCity = touristRouteFromRepo.DepartureCity.ToString()

            };
            */
            var touristRouteDto = _mapper.Map<TouristRouteDto>(touristRouteFromRepo);

            return Ok(touristRouteDto);
        }
        /// <summary>
        /// 添加TouristRoute
        /// </summary>
        /// <param name="touristRouteForCreationDto"></param>
        /// <returns></returns>
        /**
         {
            "title": "string",
            "description": "string",
            "originalPrice": 0,
            "discountPresent": 0,
            "createTime": "2021-05-01T14:05:05.631Z",
            "updateTime": "2021-05-01T14:05:05.631Z",
            "departureTime": "2021-05-01T14:05:05.631Z",
            "features": "string",
            "fees": "string",
            "notes": "string",
            "rating": 0,
            "travelDays": "string",
            "tripType": "string",
            "departureCity": "string",
            "touristRoutePictures": [
             {
               "url": "string"
             }
           ]
         }
         */
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateTouristRoute([FromBody] TouristRouteForCreationDto touristRouteForCreationDto)
        {
            var touristRouteModel = _mapper.Map<TouristRoute>(touristRouteForCreationDto);
            _touristRouteRepository.AddTouristRoute(touristRouteModel);
            await _touristRouteRepository.SaveAsync();
            var touristRouteToReture = _mapper.Map<TouristRouteDto>(touristRouteModel);
            return CreatedAtRoute("GetTouristRouteById", new { touristRouteId = touristRouteToReture.Id }, touristRouteToReture);
        }
        /// <summary>
        /// 全局更新TouristRoute
        /// </summary>
        /// <param name="touristRouteId"></param>
        /// <param name="touristRouteForUpdateDto"></param>
        /// <returns></returns>
        /**
        {
            "title": "string",
            "description": "string",
            "originalPrice": 0,
            "discountPresent": 0,
            "createTime": "2021-05-01T14:04:48.240Z",
            "updateTime": "2021-05-01T14:04:48.240Z",
            "departureTime": "2021-05-01T14:04:48.240Z",
            "features": "string",
            "fees": "string",
            "notes": "string",
            "rating": 0,
            "travelDays": "string",
            "tripType": "string",
            "departureCity": "string",
            "touristRoutePictures": [
              {
                "url": "string"
              }
            ],
            "description": "string"
            }
        */
        [HttpPut("{touristRouteId}")]
        public async Task<IActionResult> UpdateTouristRoute([FromRoute] Guid touristRouteId,[FromBody] TouristRouteForUpdateDto touristRouteForUpdateDto)
        {
            if (!await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
            {
                return NotFound("旅游路线找不到");
            }
            var touristRouteFormRepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            //1.映射dto
            //2.更新dto
            //3.映射model
            _mapper.Map(touristRouteForUpdateDto, touristRouteFormRepo);
            await _touristRouteRepository.SaveAsync();
            return NoContent();

        }
        /// <summary>
        /// 局部更新TouristRoute
        /// 
        /// </summary>
        /// <param name="touristRouteId"></param>
        /// <param name="patchDocument"></param>
        /// <returns></returns>
        /**
        [
          {
            "op": "replace",
            "path": "/title",
            "value": "abcdefg"
          },
          {
            "op": "replace",
            "path": "/description",
            "value": "abcdefgh"
          }
        ]
        */
        [HttpPatch("{touristRouteId}")]
        public async Task<IActionResult> PartiallyUpdateTouristRoute([FromRoute]Guid touristRouteId, [FromBody] JsonPatchDocument<TouristRouteForUpdateDto> patchDocument)
        {
            if (!await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
            {
                return NotFound("旅游路线找不到");
            }

            var touristRouteFromRepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            var touristRouteToPatch = _mapper.Map<TouristRouteForUpdateDto>(touristRouteFromRepo);
            patchDocument.ApplyTo(touristRouteToPatch,ModelState);
            //数据验证
            if (!TryValidateModel(touristRouteToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(touristRouteToPatch, touristRouteFromRepo);
            await _touristRouteRepository.SaveAsync();
            return NoContent();

        }
        /// <summary>
        /// 删除TouristRoute
        /// </summary>
        /// <param name="touristRouteId"></param>
        /// <returns></returns>
        [HttpDelete("{touristRouteId}")]
        public async Task<IActionResult> DeteteTouristRoute([FromRoute] Guid touristRouteId)
        {
            if (!await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
            {
                return NotFound("旅游路线找不到");
            }
            var touristRoute = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            _touristRouteRepository.DeleteTouristRoute(touristRoute);
            await _touristRouteRepository.SaveAsync();
            return NoContent();
        }
        /// <summary>
        /// 批量删除请求
        /// http://localhost:5000/api/TouristRoutes/(6db1c454-03fb-48e9-b5d0-437b9ad8e425,39996f34-013c-4fc6-b1b3-0c1036c47112)
        /// </summary>
        /// <param name="touristIDs"></param>
        /// <returns></returns>
        [HttpDelete("({touristIDs})")]
        public async Task<IActionResult> DeleteByIDs(
            [ModelBinder(BinderType = typeof(ArrayModelBinder))][FromRoute] IEnumerable<Guid> touristIDs)
        {
            if (touristIDs == null)
            {
                return BadRequest();
            }
            var touristRoutesFromRepo = await _touristRouteRepository.GetTouristRoutesByIDListAsync(touristIDs);
            _touristRouteRepository.DeleteTouristRoutes(touristRoutesFromRepo);
            await _touristRouteRepository.SaveAsync();
            return NoContent();
        }
    }
}
