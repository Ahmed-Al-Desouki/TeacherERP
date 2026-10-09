using Microsoft.EntityFrameworkCore;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Infrastructure.Data;

namespace TeacherERP.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<ApplicationUser?> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        }

        public Task<bool> PhoneNumberExistsAsync(string phoneNumber, Guid excludeUserId, CancellationToken cancellationToken)
        {
            return _context.Users.AnyAsync(u => u.PhoneNumber == phoneNumber && u.Id != excludeUserId, cancellationToken);
        }
    }
}