using AutoMapper;
using FakeXiecheng.API.Dtos;
using FakeXiecheng.API.Moldes;
using FakeXiecheng.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeXiecheng.API.Controllers
{
    
    [Route("api/touristRoutes/{touristRouteId}/pictures")]
    [ApiController]
    public class TouristRoutePicturesController : ControllerBase
    {
        private ITouristRouteRepository _touristRouteRepository;
        private IMapper _mapper;
        public TouristRoutePicturesController(ITouristRouteRepository touristRouteRepository,IMapper mapper)
        {
            _touristRouteRepository = touristRouteRepository ?? throw new ArgumentNullException(nameof(touristRouteRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        /// <summary>
        /// 获取图片列表
        /// http://localhost:5000/api/touristRoutes/0729a845-4951-4a02-bb09-0934d4d9beb0/pictures
        /// </summary>
        /// <param name="touristRouteId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetPictureListForTouristRoute(Guid touristRouteId)
        {
            if (! await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
            {
                return NotFound("旅游路线不存在");
            }
            var picturesFormRepo = await _touristRouteRepository.GetPicturesByTouristRouteIdAsync(touristRouteId);
            if (picturesFormRepo == null || picturesFormRepo.Count() <= 0)
            {
                return NotFound("照片不存在");
            }
            return Ok(_mapper.Map<IEnumerable<TouristRoutePictureDto>>(picturesFormRepo));
        }
        /// <summary>
        /// 获取图片
        /// http://localhost:5000/api/touristRoutes/0729a845-4951-4a02-bb09-0934d4d9beb0/pictures/70
        /// </summary>
        /// <param name="touristRouteId"></param>
        /// <param name="pictureId"></param>
        /// <returns></returns>
        [HttpGet("{pictureId}",Name = "GetPictrue")]
        public async Task<IActionResult> GetPictrue(Guid touristRouteId,int pictureId)
        {
            if (!await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
            {
                return NotFound("旅游线路不存在");
            }
            var pictureFromRepo = await _touristRouteRepository.GetPictureAsync(pictureId);
            if (pictureFromRepo == null)
            {
                return NotFound("相片不存在");
            }
            return Ok(_mapper.Map<TouristRoutePictureDto>(pictureFromRepo));
        }
        /// <summary>
        /// 添加图片
        /// </summary>
        /// <param name="touristRouteId"></param>
        /// <param name="touristRoutePictureForCreationDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateTouristRoutePicture([FromRoute] Guid touristRouteId,[FromBody] TouristRoutePictureForCreationDto touristRoutePictureForCreationDto)
        {
            if (!await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
            {
                return NotFound("旅游路线不存在");
            }
            var pictureModel = _mapper.Map<TouristRoutePicture>(touristRoutePictureForCreationDto);
            _touristRouteRepository.AddTouristRoutePicture(touristRouteId, pictureModel);
            await _touristRouteRepository.SaveAsync();
            var pictureToReturn = _mapper.Map<TouristRoutePictureDto>(pictureModel);
            return CreatedAtRoute("GetPictrue",
            new
            {
                touristRouteId = pictureModel.TouristRouteId,
                pictureId = pictureModel.Id
            },
            pictureToReturn
            );
        }
        /// <summary>
        /// 删除图片
        /// http://localhost:5000/api/touristRoutes/4eeb724b-6e47-4aec-9755-a3dae93c434d/pictures/69
        /// </summary>
        /// <param name="touristRouteId"></param>
        /// <param name="pictureId"></param>
        /// <returns></returns>
        [HttpDelete("{pictureId}")]
        public async Task<IActionResult> DeletePicture([FromRoute] Guid touristRouteId,[FromRoute] int pictureId)
        {
            if (!await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
            {
                return NotFound("旅游路线不存在");
            }
            var picture = await _touristRouteRepository.GetPictureAsync(pictureId);
            _touristRouteRepository.DeleteTouristRoutePicture(picture);
            await _touristRouteRepository.SaveAsync();
            return NoContent();
        }
    }
}
