using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaintyTestApi.Services;

namespace PaintyTestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly IMediaService _mediaService;
        private readonly IConfiguration _configuration;

        public MediaController(IMediaService mediaService, IConfiguration configuration)
        {
            _mediaService = mediaService;
            _configuration = configuration;
        }

        [Authorize]
        [HttpPost("Upload")]
        public async Task<ObjectResult> Upload(IFormFile file)
        {
            try
            {
                var userId = HttpContext.User.Identity?.Name;
                if (userId == null) return BadRequest("Пользователь не найден!");
                
                var basePath = _configuration["ImagePath"];
                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                var path = Path.Combine(basePath!, fileName);
                await using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                await _mediaService.AddImageAsync(userId, fileName);

                return Ok("Изображение загружено!");
            }
            catch (Exception e)
            {
                return BadRequest("Произошла ошибка при загрузке!"); 
            }
        }

        [Authorize]
        [HttpGet("GetUserImages")]
        public async Task<ObjectResult> GetUserImages()
        {
            try
            {
                var userId = HttpContext.User.Identity?.Name;
                if (userId == null) return BadRequest("Пользователь не найден!");

                var images = await _mediaService.GetUserImagesAsync(userId);

                if (images == null) return BadRequest("Пользователь не найден!");

                if (images.Count == 0) return Ok("У пользователя нет фотографий!");

                return Ok(images);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize]
        [HttpGet("DownloadFriendImages")]
        public async Task<ObjectResult> DownloadFriendImages(string friendId)
        {
            try
            {
                var userId = HttpContext.User.Identity?.Name;
                if (userId == null) return BadRequest("Пользователь не найден!");

                var images = await _mediaService.GetFriendImagesAsync(userId, friendId);

                if (images == null) return BadRequest("Пользователь не найден, либо у вас не установлен контакт!!");

                if (images.Count == 0) return Ok("У пользователя нет фотографий!");

                return Ok(images);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize]
        [HttpPost("GetImage")]
        public async Task<IActionResult> GetImage(string fileName)
        {
            try
            {
                var basePath = _configuration["ImagePath"];
                var bytes = await System.IO.File.ReadAllBytesAsync(Path.Combine(basePath!, $"{fileName}"));
                return File(bytes, "image/jpeg");
            }
            catch (Exception e)
            {
                return BadRequest("Ошибка при загрузке файла!");
            }
        }
    }
}
