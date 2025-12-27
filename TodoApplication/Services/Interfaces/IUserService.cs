using TodoApplication.Dto;
using TodoApplication.Dto.User;
using TodoApplication.Entities;

namespace TodoApplication.Services.Interfaces;

public interface IUserService
{
   
    //Task<Response> UpdateUserAsync(Guid id, UserUpdateDto dto, CancellationToken ct);
    Task<Response> UserDeleteAsync(Guid id,  CancellationToken ct);
    Task<Response> BlockUnBlockUser(Guid id, CancellationToken ct);
    Task<Response> ResetSendConfirmEmailOTP(Guid user_id, CancellationToken ct);
    Task<List<UserListDto>>  GetAllUsersAsync(CancellationToken ct);
    Task<UserDetailDto?> GetUserByIdAsync(Guid id, CancellationToken ct);
    Task<Response> AdminAddUserAsync(AdminAddUserDto dto, CancellationToken ct);
    Task<Response> AdminUpdateUserAsync(Guid id, AdminUpdateUserDto dto, CancellationToken ct);
    Task<Response> ChangePassword(Guid user_id, ChangePasswordDto dto, CancellationToken ct);


}