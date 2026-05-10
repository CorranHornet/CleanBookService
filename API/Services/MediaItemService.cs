using Microsoft.EntityFrameworkCore;
using BookService.Data;
using BookService.Dtos;
using BookService.Models;

namespace BookService.Services
{
    public class MediaItemService : IMediaItemService
    {
        private readonly AppDbContext _context;

        public MediaItemService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MediaItemResponseDTO>> GetAllAsync(string? search = null)
        {
            var query = _context.MediaItems
                .Include(m => m.Genre)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(t =>
                    t.Title != null &&
                    t.Title.ToLower().Contains(search.ToLower()));
            }

            return await query
                .Select(t => new MediaItemResponseDTO
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Genre = t.Genre.Name, // FIX: now comes from navigation property
                    Creator = t.Creator,
                    ReleaseDate = t.ReleaseDate,
                    ScheduledDate = t.ScheduledDate,
                    PageCount = t.PageCount,
                    DurationMinutes = t.DurationMinutes,
                    TrackCount = t.TrackCount,
                    Publisher = t.Publisher,
                    Language = t.Language,
                    MediaType = t.MediaType
                })
                .ToListAsync();
        }

        public async Task<MediaItemResponseDTO?> GetByIdAsync(int id)
        {
            return await _context.MediaItems
                .Include(t => t.Genre)
                .Where(t => t.Id == id)
                .Select(t => new MediaItemResponseDTO
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Genre = t.Genre.Name,
                    Creator = t.Creator,
                    ReleaseDate = t.ReleaseDate,
                    ScheduledDate = t.ScheduledDate,
                    PageCount = t.PageCount,
                    DurationMinutes = t.DurationMinutes,
                    TrackCount = t.TrackCount,
                    Publisher = t.Publisher,
                    Language = t.Language,
                    MediaType = t.MediaType
                })
                .FirstOrDefaultAsync();
        }

        public async Task<MediaItemResponseDTO> CreateAsync(MediaItemCreateDTO dto)
        {
            // Find or create Genre
            Genre? genre = null;

            if (!string.IsNullOrWhiteSpace(dto.Genre))
            {
                genre = await _context.Genres
                    .FirstOrDefaultAsync(g => g.Name.ToLower() == dto.Genre.ToLower());

                if (genre == null)
                {
                    genre = new Genre { Name = dto.Genre };
                    _context.Genres.Add(genre);
                    await _context.SaveChangesAsync();
                }
            }

            var mediaItem = new MediaItem
            {
                Title = dto.Title,
                Description = dto.Description,
                GenreId = genre?.Id ?? 0,   // FIX: FK used instead of string
                Creator = dto.Creator,
                ReleaseDate = dto.ReleaseDate,
                ScheduledDate = dto.ScheduledDate,
                PageCount = dto.PageCount,
                DurationMinutes = dto.DurationMinutes,
                TrackCount = dto.TrackCount,
                Publisher = dto.Publisher,
                Language = dto.Language,
                MediaType = dto.MediaType
            };

            _context.MediaItems.Add(mediaItem);
            await _context.SaveChangesAsync();

            return new MediaItemResponseDTO
            {
                Id = mediaItem.Id,
                Title = mediaItem.Title,
                Description = mediaItem.Description,
                Genre = genre?.Name,
                Creator = mediaItem.Creator,
                ReleaseDate = mediaItem.ReleaseDate,
                ScheduledDate = mediaItem.ScheduledDate,
                PageCount = mediaItem.PageCount,
                DurationMinutes = mediaItem.DurationMinutes,
                TrackCount = mediaItem.TrackCount,
                Publisher = mediaItem.Publisher,
                Language = mediaItem.Language,
                MediaType = mediaItem.MediaType
            };
        }

        public async Task<bool> UpdateAsync(int id, MediaItemUpdateDTO dto)
        {
            var mediaItem = await _context.MediaItems
                .Include(m => m.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mediaItem == null)
                return false;

            if (!string.IsNullOrWhiteSpace(dto.Title))
                mediaItem.Title = dto.Title;

            if (dto.ReleaseDate.HasValue)
                mediaItem.ReleaseDate = dto.ReleaseDate.Value;

            if (dto.ScheduledDate.HasValue)
                mediaItem.ScheduledDate = dto.ScheduledDate.Value;

            if (!string.IsNullOrWhiteSpace(dto.Description))
                mediaItem.Description = dto.Description;

            if (!string.IsNullOrWhiteSpace(dto.Creator))
                mediaItem.Creator = dto.Creator;

            if (dto.PageCount.HasValue)
                mediaItem.PageCount = dto.PageCount.Value;

            if (dto.DurationMinutes.HasValue)
                mediaItem.DurationMinutes = dto.DurationMinutes.Value;

            if (dto.TrackCount.HasValue)
                mediaItem.TrackCount = dto.TrackCount.Value;

            if (!string.IsNullOrWhiteSpace(dto.Publisher))
                mediaItem.Publisher = dto.Publisher;

            if (!string.IsNullOrWhiteSpace(dto.Language))
                mediaItem.Language = dto.Language;

            if (!string.IsNullOrWhiteSpace(dto.MediaType))
                mediaItem.MediaType = dto.MediaType;

            // FIX: Genre update logic
            if (!string.IsNullOrWhiteSpace(dto.Genre))
            {
                var genre = await _context.Genres
                    .FirstOrDefaultAsync(g => g.Name.ToLower() == dto.Genre.ToLower());

                if (genre == null)
                {
                    genre = new Genre { Name = dto.Genre };
                    _context.Genres.Add(genre);
                    await _context.SaveChangesAsync();
                }

                mediaItem.GenreId = genre.Id;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var hasUnits = await _context.MediaUnits
                .AnyAsync(mu => mu.MediaItemId == id);

            if (hasUnits)
                return false;

            var mediaItem = await _context.MediaItems.FindAsync(id);
            if (mediaItem == null)
                return false;

            _context.MediaItems.Remove(mediaItem);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}