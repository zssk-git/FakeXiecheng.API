﻿using FakeXiecheng.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        public AutheticateController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /**
        * {
            "email": "zssk@163.com",
            "password": "123456"
           }
        */
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            //1.验证用户名密码

            //2.创建jwt
            //header
            var signingAlgorithm = SecurityAlgorithms.HmacSha256;
            //payload
            var claims = new[]
            {
                // sub
                new Claim(JwtRegisteredClaimNames.Sub,"fake_user_id"),
                new Claim(ClaimTypes.Role,"Admin")
            };
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
    }
}
