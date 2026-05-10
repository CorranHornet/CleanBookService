using Microsoft.EntityFrameworkCore;
using BookService.Data;
using BookService.Dtos;
using BookService.Models;

namespace BookService.Services
{
    public class MediaUnitService : IMediaUnitService
    {
        private readonly AppDbContext _context;

        public MediaUnitService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MediaUnitResponseDTO>> GetAllAsync(int? mediaItemId = null)
        {
            var query = _context.MediaUnits.AsQueryable();

            if (mediaItemId.HasValue)
                query = query.Where(g => g.MediaItemId == mediaItemId.Value);

            return await query
                .Select(g => new MediaUnitResponseDTO
                {
                    Id = g.Id,
                    Title = g.Title,
                    Number = g.Number,
                    DurationMinutes = g.DurationMinutes,
                    MediaItemId = g.MediaItemId
                })
                .ToListAsync();
        }

        public async Task<MediaUnitResponseDTO?> GetByIdAsync(int id)
        {
            return await _context.MediaUnits
                .Where(g => g.Id == id)
                .Select(g => new MediaUnitResponseDTO
                {
                    Id = g.Id,
                    Title = g.Title,
                    Number = g.Number,
                    DurationMinutes = g.DurationMinutes,
                    MediaItemId = g.MediaItemId
                })
                .FirstOrDefaultAsync();
        }

        public async Task<MediaUnitResponseDTO?> CreateAsync(MediaUnitCreateDTO dto)
        {
            
            var mediaItemExists = await _context.MediaItems
                .AnyAsync(x => x.Id == dto.MediaItemId);

            if (!mediaItemExists)
                return null;

            var mediaUnit = new MediaUnit
            {
                Title = dto.Title,
                Number = dto.Number,
                DurationMinutes = dto.DurationMinutes,
                MediaItemId = dto.MediaItemId
            };

            _context.MediaUnits.Add(mediaUnit);
            await _context.SaveChangesAsync();

            return new MediaUnitResponseDTO
            {
                Id = mediaUnit.Id,
                Title = mediaUnit.Title,
                Number = mediaUnit.Number,
                DurationMinutes = mediaUnit.DurationMinutes,
                MediaItemId = mediaUnit.MediaItemId
            };
        }

        public async Task<bool> UpdateAsync(int id, MediaUnitUpdateDTO dto)
        {
            var mediaUnit = await _context.MediaUnits.FindAsync(id);
            if (mediaUnit == null) return false;

            if (!string.IsNullOrWhiteSpace(dto.Title)) mediaUnit.Title = dto.Title;
            if (dto.Number.HasValue) mediaUnit.Number = dto.Number.Value;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var mediaUnit = await _context.MediaUnits.FindAsync(id);
            if (mediaUnit == null) return false;

            _context.MediaUnits.Remove(mediaUnit);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}