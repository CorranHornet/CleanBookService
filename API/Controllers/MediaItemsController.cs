using Microsoft.AspNetCore.Mvc;
using BookService.Services;
using BookService.Dtos;

namespace BookService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MediaItemsController : ControllerBase
    {
        private readonly IMediaItemService _service;
        public MediaItemsController(IMediaItemService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string? search)
        {
            var mediaItems = await _service.GetAllAsync(search);
            return Ok(mediaItems);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var mediaItem = await _service.GetByIdAsync(id);

                if (mediaItem == null)
                    return NotFound();

            return Ok(mediaItem);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MediaItemCreateDTO dto)
        {
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, MediaItemUpdateDTO dto)
        {
            var success = await _service.UpdateAsync(id, dto);

            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);

            if (!success)
                return BadRequest (new { message = "Cannot delete MediaItem with existing MediaUnits."});

            return NoContent();
        }
    }
}