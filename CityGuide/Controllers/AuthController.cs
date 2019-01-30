using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CityGuide.Data;
using CityGuide.Dtos;
using CityGuide.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CityGuide.Controllers
{
    [Produces("application/json")]
    [Route("api/Auth")]
    public class AuthController : Controller
    {
        private IAuthRepository _authRepository;
        private IConfiguration _configuration;
        public AuthController(IAuthRepository authRepository,IConfiguration configuration)
        {
            _authRepository = authRepository;
            _configuration = configuration;
        }


        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserForRegiterDto userForRegiterDto)
        {
            if (await _authRepository.UserExists(userForRegiterDto.UserName))
            {
                ModelState.AddModelError("UserName", "User name already exist");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userToCreate = new User()
            {
                UserName = userForRegiterDto.UserName
            };

            var createdUser = await _authRepository.Register(userToCreate, userForRegiterDto.Password);
            return StatusCode(201);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserForLoginDto userForLoginDto)
        {
            var user = await _authRepository.Login(userForLoginDto.UserName, userForLoginDto.Password);

            if (user==null)
            {
                return Unauthorized();
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:Token").Value);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
                }),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials= new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(tokenString);
        }


    }
}