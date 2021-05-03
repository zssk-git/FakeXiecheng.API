﻿using FakeXiecheng.API.Dtos;
using FakeXiecheng.API.Moldes;
using FakeXiecheng.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FakeXiecheng.API.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AutheticateController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITouristRouteRepository _touristRouteRepository;
        public AutheticateController(IConfiguration configuration,
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager,
            ITouristRouteRepository touristRouteRepositor)
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _touristRouteRepository = touristRouteRepositor;
        }
        /**
        * {
            "email": "zssk@163.com",
            "password": "123456"
           }
        */
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            //1.验证用户名密码
            var loginResult = await _signInManager.PasswordSignInAsync(
                loginDto.Email,
                loginDto.Password,
                false,
                false
                );
            if (!loginResult.Succeeded)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByNameAsync(loginDto.Email);
            //2.创建jwt
            //header
            var signingAlgorithm = SecurityAlgorithms.HmacSha256;
            //payload
            var claims = new List<Claim>
            {
                // sub
                new Claim(JwtRegisteredClaimNames.Sub,user.Id),
                //new Claim(ClaimTypes.Role,"Admin")

            };
            var roleNames = await _userManager.GetRolesAsync(user);
            foreach (var roleName in roleNames)
            {
                var roleClaim = new Claim(ClaimTypes.Role, roleName);
                claims.Add(roleClaim);
            }
            //signiture
            var secretByte = Encoding.UTF8.GetBytes(_configuration["Authentication:Secretkey"]);
            var signingkey = new SymmetricSecurityKey(secretByte);
            var signingCredentials = new SigningCredentials(signingkey, signingAlgorithm);

            var token = new JwtSecurityToken(
                issuer: _configuration["Authentication:Issuer"],
                audience: _configuration["Authentication:Audience"],
                claims,
                notBefore:DateTime.UtcNow,
                expires:DateTime.UtcNow.AddDays(1),
                signingCredentials
            );
            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
            //3 return 200 ok + jwt
            return Ok(tokenStr);
        }
        /**
         * {
            "email": "zssk1@163.com",
            "password": "Zssk@123456",
            "confirmPassword": "Zssk@123456"
           }
         */
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            //1.使用用户名创建用户对象
            var user = new ApplicationUser()
            {
                UserName = registerDto.Email,
                Email = registerDto.Email
            };
            //2.hash密码，保存用户
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest();
            }

            //3.初始化购物车
            var shoppingCart = new ShoppingCart
            {
                Id = Guid.NewGuid(),
                UserId = user.Id
            };
            await _touristRouteRepository.CreateShoppingCart(shoppingCart);
            await _touristRouteRepository.SaveAsync();
            //4.return
            return Ok();
        }
    }
}
