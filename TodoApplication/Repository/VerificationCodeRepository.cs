using Microsoft.EntityFrameworkCore;
using TodoApplication.Data;
using TodoApplication.Entities;
using TodoApplication.Repository.Interfaces;

namespace TodoApplication.Repository;

public class VerificationCodeRepository : IVerificationCodeRepository
{
    private readonly TodoAppDbContext _context;
    public VerificationCodeRepository(TodoAppDbContext context)
    {
        _context = context;
    }
    public async Task AddAsync(VerificationCode token, CancellationToken ct)
    {
        await _context.VerificationCodes.AddAsync(token);
        await _context.SaveChangesAsync(ct);
    }


    public async Task<VerificationCode?> GetValidCodeAsync(byte[] codeHash)
    {
        return await _context.VerificationCodes
            .FirstOrDefaultAsync(x =>
                x.codehash == codeHash &&
                x.used_at == null &&
                x.expires_at > DateTime.UtcNow);
    }


    public async Task UpdateAsync(VerificationCode token)
    {
        _context.VerificationCodes.Update(token);
        await _context.SaveChangesAsync();
    }
}