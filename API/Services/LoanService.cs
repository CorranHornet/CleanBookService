using BookService.Data;
using BookService.Dtos;
using BookService.Models;
using Microsoft.EntityFrameworkCore;

namespace BookService.Services
{
    public class LoanService : ILoanService
    {
        private readonly AppDbContext _context;
        public LoanService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> BorrowAsync(int userId, int mediaUnitId)
        {
            var mediaUnitExists = await _context.MediaUnits
                .AnyAsync(mu => mu.Id == mediaUnitId);

            var userExists = await _context.Users
                .AnyAsync(u => u.Id == userId);

            if (!mediaUnitExists || !userExists)
                return false;

            
            
            
            // check if already Loaned
            var alreadyLoaned = await _context.Loans
                .AnyAsync(l => l.MediaUnitId == mediaUnitId && l.ReturnDate == null);

            if (alreadyLoaned)
                return false;






            var loan = new Loan
            {
                MediaUnitId = mediaUnitId,
                UserId = userId,
                LoanDate = DateTime.Now
            };

            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> ReturnAsync(int loanId)
        {
            var loan = await _context.Loans
                .FirstOrDefaultAsync(l => l.Id == loanId);

            if (loan == null || loan.ReturnDate != null)
                return false;

            loan.ReturnDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return true;

            
        }

        public async Task<IEnumerable<LoanResponseDTO>> GetAllAsync()
        {
            return await _context.Loans
                .Include(l => l.User)
                .Include(l => l.MediaUnit)
                .Select(l => new LoanResponseDTO
                {
                    Id = l.Id,
                    LoanDate = l.LoanDate,
                    ReturnDate = l.ReturnDate,

                    User = new UserDTO
                    {
                    Id = l.User.Id,
                    Username = l.User.Username,
                    Email =  l.User.Email
                    },

                    MediaUnit = new MediaUnitDTO
                    {
                        Id = l.MediaUnit.Id,
                        Title = l.MediaUnit.Title,
                        Number = l.MediaUnit.Number,
                        MediaItemId = l.MediaUnit.MediaItemId
                    }

                })
                .ToListAsync();
        }
    }
}

