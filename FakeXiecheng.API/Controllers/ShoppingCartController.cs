using AutoMapper;
using FakeXiecheng.API.Dtos;
using FakeXiecheng.API.Moldes;
using FakeXiecheng.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FakeXiecheng.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITouristRouteRepository _touristRouteRepository;
        private readonly IMapper _mapper;
        
        public ShoppingCartController(IHttpContextAccessor httpContextAccessor, 
            ITouristRouteRepository touristRouteRepository,
            IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _touristRouteRepository = touristRouteRepository;
            _mapper = mapper;
        }
        /// <summary>
        /// 获取当前用户购物车信息
        /// http://localhost:5000/api/shoppingcart
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetShoppingCart()
        {
            //1 获得当前用户
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            //2 使用userid获取购物车
            var shoppingCart = await _touristRouteRepository.GetShoppingCartByUserId(userId);

            return Ok(_mapper.Map<ShoppingCartDto>(shoppingCart));

        }
        /// <summary>
        /// 向购物车加入商品
        /// http://localhost:5000/api/shoppingcart/items
        /// </summary>
        /// <param name="addShoppingCartItemDto"></param>
        /// <returns></returns>
        /**
         {
            "touristRouteId":"06f8370d-52f2-4766-bf38-f115cd62dc97"
         }
         * */
        [HttpPost("items")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> AddShppingCartItem([FromBody]AddShoppingCartItemDto addShoppingCartItemDto)
        {
            //1 获得当前用户
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            //2 使用userid获取购物车
            var shoppingCart = await _touristRouteRepository.GetShoppingCartByUserId(userId);

            //3 创建lineItem
            var touristRoute = await _touristRouteRepository.GetTouristRouteAsync(addShoppingCartItemDto.TouristRouteId);
            if (touristRoute == null)
            {
                return NotFound("旅游路线不存在");
            }
            var lineItem = new LineItem()
            {
                TouristRouteId = addShoppingCartItemDto.TouristRouteId,
                ShoppingCartId = shoppingCart.Id,
                OriginalPrice = touristRoute.OriginalPrice,
                DiscountPresent = touristRoute.DiscountPresent
            };
            //4 添加lineitem,并保存数据库
            await _touristRouteRepository.AddShoppingCartItem(lineItem);
            await _touristRouteRepository.SaveAsync();
            return Ok(_mapper.Map<ShoppingCartDto>(shoppingCart));
        }
        /// <summary>
        /// 从购物车删除商品
        /// http://localhost:5000/api/shoppingcart/items/1
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [HttpDelete("items/{itemId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> DeleteShoppingCartItem([FromRoute] int itemId)
        {
            //1 获取lineitem数据
            var lineItem = await _touristRouteRepository.GetShoppingCartIntemByItemId(itemId);
            if (lineItem == null)
            {
                return NotFound("购物车商品找不到");
            }
            _touristRouteRepository.DeleteShoppingCartItem(lineItem);
            await _touristRouteRepository.SaveAsync();

            return NoContent();
        }
    }
}
