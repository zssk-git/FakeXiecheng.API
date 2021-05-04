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
    public class OrdersController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITouristRouteRepository _touristRouteRepository;
        private readonly IMapper _mapper;
        public OrdersController(IHttpContextAccessor httpContextAccessor,ITouristRouteRepository touristRouteRepository,IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _touristRouteRepository = touristRouteRepository;
            _mapper = mapper;
        }
        /// <summary>
        /// 获取用户订单
        /// http://localhost:5000/api/orders/
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetOrders()
        {
            //1. 获取当前用户
            var userID = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //2.使用用户id来获取订单历史记录
            var orders = await _touristRouteRepository.GetOrdersByUserId(userID);

            return Ok(_mapper.Map<IEnumerable<OrderDto>>(orders));
        }
        /// <summary>
        /// 获取订单详细
        /// http://localhost:5000/api/orders/70dff164-4028-4071-81cf-88be2f9313d6
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpGet("{orderId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetOrderById([FromRoute] Guid orderId)
        {
            //1. 获取当前用户
            var userID = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var order = await _touristRouteRepository.GetOrderById(orderId);
            return Ok(_mapper.Map<OrderDto>(order));
        }

    }
}
