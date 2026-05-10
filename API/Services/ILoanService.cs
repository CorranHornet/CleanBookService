using BookService.Dtos;
using BookService.Models;

namespace BookService.Services
{
    public interface ILoanService
    {
        Task<bool> BorrowAsync(int userId, int mediaUnitId);
        Task<bool> ReturnAsync(int loanId);
        Task<IEnumerable<LoanResponseDTO>> GetAllAsync();
    }
}
