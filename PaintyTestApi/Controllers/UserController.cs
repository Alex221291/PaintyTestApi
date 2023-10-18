using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaintyTestApi.Services;

namespace PaintyTestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpGet("GetUsers")]
        public async Task<ObjectResult> GetUsers()
        {
            try
            {
                var userId = HttpContext.User.Identity?.Name!;
                var result = await _userService.GetUsersAsync(userId);
                if (result == null) return BadRequest("Неверные параметры!");
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize]
        [HttpPost("AddFriend")]
        public async Task<ObjectResult> AddFriend(string friendId)
        {
            try
            {
                var userId = HttpContext.User.Identity?.Name!;
                var result = await _userService.AddFiendAsync(userId, friendId);

                if (result == null) return NotFound("Пользователь не найден!");

                return (bool)result ? Ok("Пользователь добавлен в друзья!") : BadRequest("Пользователь уже у вас в друзьях!");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize]
        [HttpGet("GetFriends")]
        public async Task<ObjectResult> GetFriends()
        {
            try
            {
                var userId = HttpContext.User.Identity?.Name!;
                var result = await _userService.GetFiendsAsync(userId);
                if (result == null) return NotFound("Пользователь не найден!");
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
