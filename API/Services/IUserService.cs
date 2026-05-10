using BookService.Dtos;

namespace BookService.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDTO>> GetAllAsync();
        Task<UserResponseDTO?> GetByIdAsync(int id);
        Task<UserResponseDTO> CreateAsync(UserCreateDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
