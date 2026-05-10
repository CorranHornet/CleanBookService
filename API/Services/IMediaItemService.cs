using BookService.Dtos;

namespace BookService.Services
{
    public interface IMediaItemService
    {
        Task<IEnumerable<MediaItemResponseDTO>> GetAllAsync(string? search = null);
        Task<MediaItemResponseDTO?> GetByIdAsync(int id);
        Task<MediaItemResponseDTO> CreateAsync(MediaItemCreateDTO dto);
        Task<bool> UpdateAsync(int id, MediaItemUpdateDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
