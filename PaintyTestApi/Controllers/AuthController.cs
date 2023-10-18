using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PaintyTestApi.Auth;
using PaintyTestApi.Services;
using PaintyTestApi.ViewModels.AuthViewModels;

namespace PaintyTestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Reg")]
        public async Task<ObjectResult> Reg(LoginViewModel model)
        {

            if (model.Password.Length <= 7)
            {
                return BadRequest("Пароль должен содержать минимум 8 символов!");
            }

            if (await _authService.CheckLoginAsync(model.Login))
            {
                return BadRequest("Пользователь с таким логином уже существует!");
            }

            var user = await _authService.RegAsync(model);

            var identity = GetIdentity(user.Id);
            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromHours(10)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            user.Token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return Ok(user);
        }

        [HttpGet("Auth")]
        public async Task<ObjectResult> Auth([FromQuery] LoginViewModel model)
        {
            var user = await _authService.AuthAsync(model);

            if (user == null) return BadRequest("Неверный email или пароль!");

            var identity = GetIdentity(user.Id);
            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromHours(10)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            user.Token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return Ok(user);
        }

        private static ClaimsIdentity GetIdentity(string id)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.UniqueName, id),
            };

            var claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }
    }
}
