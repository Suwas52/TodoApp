using TodoApplication.Entities;

namespace TodoApplication.Repository.Interfaces;

public interface IVerificationCodeRepository
{
    Task AddAsync(VerificationCode token, CancellationToken ct);
    Task<VerificationCode?> GetValidCodeAsync(byte[] codeHash);
    Task UpdateAsync(VerificationCode token);

}