﻿using Authentication_With_Angular_Net7.Configurations;
using Authentication_With_Angular_Net7.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Authentication_With_Angular_Net7.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthManagementController : ControllerBase
{

    private readonly ILogger<AuthManagementController> _logger;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly JWTConfig _jWTConfig;
    public AuthManagementController(ILogger<AuthManagementController> logger, UserManager<IdentityUser> userManager, IOptionsMonitor<JWTConfig> optionsMonitor)
    {
        _logger = logger;
        _userManager = userManager;
        _jWTConfig = optionsMonitor.CurrentValue;
    }

    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDTO registerDTO)
    {
        if(ModelState.IsValid)
        {
            // check if email exist
            var emailExist = await _userManager.FindByEmailAsync(registerDTO.Email);    
            if (emailExist != null)
                return BadRequest("Email already exist");
            var newUser = new IdentityUser()
            {
                Email = registerDTO.Email,
                UserName = registerDTO.Email
            };
            var isCreated = await _userManager.CreateAsync(newUser, registerDTO.Password);
            if(isCreated.Succeeded)
            {
                var token = GenerateJwtToken(newUser);
                //Generate token
                return Ok(new RegisterRequestReponse()
                {
                    Result = true,
                    Token = token
                }) ;
            }
            return BadRequest(isCreated.Errors.Select(x=>x.Description).ToList());
        }
        return BadRequest("Invalid request payload");
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDTO loginDTO )
    {
        if (ModelState.IsValid)
        {
            var existingUser = await _userManager.FindByEmailAsync(loginDTO.Email);
            if(existingUser == null)
            {
                return BadRequest("Invalid Authentication");
            }
            var isPasswordValid = await _userManager.CheckPasswordAsync(existingUser, loginDTO.Password);
            if (isPasswordValid)
            {
                var token = GenerateJwtToken(existingUser);
                return Ok(new LoginRequestReponse()
                {
                    Token = token,
                    Result = true
                });
            }
            return BadRequest("Invalid Authentication");
        }
        return BadRequest("Invalid Authentication");
    }
    private string GenerateJwtToken(IdentityUser user)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jWTConfig.secret);
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new[]
            {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                     new Claim(JwtRegisteredClaimNames.Email, user.Email),
                     new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())

                }),
            Expires = DateTime.UtcNow.AddHours(4),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512)
        };
        var token = jwtTokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = jwtTokenHandler.WriteToken(token);
        return jwtToken;
    }

   
}
