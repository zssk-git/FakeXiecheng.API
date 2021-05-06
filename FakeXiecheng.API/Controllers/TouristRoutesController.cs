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
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
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
        private readonly IUrlHelper _urlHelper;
        private readonly IPropertyMappingService _propertyMappingService;

        public TouristRoutesController(
            ITouristRouteRepository touristRouteRepository,
            IMapper mapper, 
            IUrlHelperFactory urlHelperFactory,
            IActionContextAccessor actionContextAccessor,
            IPropertyMappingService propertyMappingService)
        {
            _touristRouteRepository = touristRouteRepository;
            _mapper = mapper;
            _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
            _propertyMappingService = propertyMappingService;
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
        //分页导航URL
        private string GenerateTouristRouteResoutceURL(
            TouristRouteResourceParameters trrParameters,
            PaginationResourceParamaters pgrParamaters,
            ResourceUrlType type)
        {
            return type switch
            {
                ResourceUrlType.PreviousPage => _urlHelper.Link("GetTouristRoutes",
                new
                {
                    fileds = trrParameters.Fields,
                    orderBy = trrParameters.OrderBy,
                    keyword = trrParameters.Keyword,
                    rating = trrParameters.Rating,
                    pageNumber = pgrParamaters.PageNumber - 1,
                    pageSize = pgrParamaters.PageSize
                }),
                ResourceUrlType.NextPage => _urlHelper.Link("GetTouristRoutes",
               new
               {
                   fileds = trrParameters.Fields,
                   orderBy = trrParameters.OrderBy,
                   keyword = trrParameters.Keyword,
                   rating = trrParameters.Rating,
                   pageNumber = pgrParamaters.PageNumber + 1,
                   pageSize = pgrParamaters.PageSize
               }),
                _ => _urlHelper.Link("GetTouristRoutes",
               new
               {
                   fileds = trrParameters.Fields,
                   orderBy = trrParameters.OrderBy,
                   keyword = trrParameters.Keyword,
                   rating = trrParameters.Rating,
                   pageNumber = pgrParamaters.PageNumber - 1,
                   pageSize = pgrParamaters.PageSize
               }),

            };
        }
        /// <summary>
        ///  获取所有TouristRoutes
        ///  api/touristRoutes?keyword=传入的参数&rating=largerThan2
        ///  http://localhost:5000/api/TouristRoutes?Keyword=埃及&RatingOperator=largerThan&RatingValue=3
        ///  http://localhost:5000/api/TouristRoutes?Keyword=埃及&Rating=largerThan4
        ///  http://localhost:5000/api/TouristRoutes?Rating=largerThan4
        ///  http://localhost:5000/api/TouristRoutes?keyword=埃及&pagesize=5&pagenumber=1
        ///  http://localhost:5000/api/TouristRoutes?orderby=originalPrice
        ///  http://localhost:5000/api/TouristRoutes?orderby=originalPrice desc
        ///  http://localhost:5000/api/TouristRoutes?orderby=rating desc ,originalPrice  desc
        ///  http://localhost:5000/api/TouristRoutes?pagesize=5&pagenumber=3&rating=largerThan2 desc &orderby=originalprice desc
        ///  http://localhost:5000/api/TouristRoutes?fields=id,title
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpGet(Name = "GetTouristRoutes")]
        public async Task<IActionResult> GetTouristRoutes(
            [FromQuery] TouristRouteResourceParameters trrParameters,
            [FromQuery] PaginationResourceParamaters pgrParamaters)
        {

            if (!_propertyMappingService.IsMappingExists<TouristRouteDto,TouristRoute>(trrParameters.OrderBy))
            {
                return BadRequest("请输入正确的排序参数");
            }
            if (!_propertyMappingService.IsPropertiesExists<TouristRouteDto>(trrParameters.Fields))
            {
                return BadRequest("请输入正确的塑性参数");
            } 

            var touristRoutesFromRepo = await _touristRouteRepository.GetTouristRoutesAsync(
                trrParameters.Keyword,
                trrParameters.RatingOperator,
                trrParameters.RatingValue,
                pgrParamaters.PageSize,
                pgrParamaters.PageNumber,
                trrParameters.OrderBy
                
                );
            if (touristRoutesFromRepo == null || touristRoutesFromRepo.Count() <= 0)
            {
                return NotFound("没有旅游路线");
            }
            var touristRoutesDto = _mapper.Map<IEnumerable<TouristRouteDto>>(touristRoutesFromRepo);

            var previousPageLink = touristRoutesFromRepo.HasPrevious
                ? GenerateTouristRouteResoutceURL(trrParameters, pgrParamaters, ResourceUrlType.PreviousPage)
                : null;
            var nextPageLink = touristRoutesFromRepo.HasNext
                ? GenerateTouristRouteResoutceURL(trrParameters, pgrParamaters, ResourceUrlType.NextPage)
                : null;
            //x-pagination
            var paginationMetadata = new
            {
                previousPageLink,
                nextPageLink,
                totalCout = touristRoutesFromRepo.TotalCount,
                pageSize = touristRoutesFromRepo.PageSize,
                currentPage = touristRoutesFromRepo.CurrentPage,
                totalPages = touristRoutesFromRepo.TotalPage
            };
            Response.Headers.Add("x-pagination", Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

            return Ok(touristRoutesDto.ShapeData(trrParameters.Fields));
        }
        /// <summary>
        /// 通过Id获取TouristRoute
        /// api/touristRoutes/{touristRouteId}
        /// http://localhost:5000/api/TouristRoutes/39996f34-013c-4fc6-b1b3-0c1036c47112
        /// http://localhost:5000/api/TouristRoutes/06f8370d-52f2-4766-bf38-f115cd62dc97?fileds=id,title
        /// </summary>
        /// <param name="touristRouteId"></param>
        /// <returns></returns>
        [HttpGet("{touristRouteId}", Name = "GetTouristRouteById")]
        public async Task<IActionResult> GetTouristRouteById(Guid touristRouteId,string fileds)
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

            return Ok(touristRouteDto.ShapeData(fileds));
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
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        //[Authorize]
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
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
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
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
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
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
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
