using System.Linq.Expressions;
using HotelBooking.application.Helpers;
using HotelBooking.application.Services;
using HotelBooking.infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public interface IUserService
{
    //admin

    public Task<User?> GetByIdAsync(int id);
    public Task<RegisterResponseDTO> RegisterAdmin(RegisterAdminDTO newAdmin);
    public Task<RegisterResponseDTO> RegisterCustomer(RegisterCustomerDTO newCustomer);
    public Task<LoginResponseDTO> LoginUser(LoginUserDTO userLogin);
    public Task<bool> ApproveUpgradeToOwnerAsync(int requestId, int adminId);
    // public Task<bool> RejectUpgradeToOwnerAsync(int requestId, int adminId);
    // user
    public Task<UserProfileDTO> GetUserProfileAsync(int userId);
    public Task<bool> UpdateUserProfileAsync(int userId, UserProfileDTO updateReq);
    public Task<bool> UpdateAvatarUrlAsync(int userId, string newAvatarUrl);
}

public class UserService : IUserService
{
    public HotelBookingContext _context;
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUpgradeRequestRepository _upgradeRequestRepository;
    public JwtAuthService _jwtAuthService;
    public IUnitOfWork _dbu;

    private IFileUploadService _uploadService;
    public UserService(HotelBookingContext context, IUserRepository userRepository, IUserRoleRepository userRoleRepository, IUpgradeRequestRepository upgradeRequestRepository, JwtAuthService jwtAuthService, IFileUploadService uploadService, IUnitOfWork dbu)
    {
        _context = context;
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _upgradeRequestRepository = upgradeRequestRepository;
        _jwtAuthService = jwtAuthService;
        _uploadService = uploadService;
        _dbu = dbu;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id && (u.IsDeleted == false));
        // lọc IsDeleted = false nếu bạn đang dùng soft delete
    }

    public async Task<RegisterResponseDTO> RegisterAdmin(RegisterAdminDTO newAdmin)
    {
        try
        {
            var checkAdmin = await _userRepository.SingleOrDefaultAsync(admin => admin.Email == newAdmin.Email || admin.UserName == newAdmin.Username);
            if (checkAdmin != null)
            {
                return new RegisterResponseDTO
                {
                    IsSuccess = false,
                    Message = checkAdmin.UserName == newAdmin.Username ? MessageRegister.USERNAME_EXIST : MessageRegister.EMAIL_EXIST
                };
            }
            ;

            var user = new User
            {
                UserName = newAdmin.Username,
                FullName = newAdmin.FullName,
                Email = newAdmin.Email,
                PhoneNumber = newAdmin.PhoneNumber,
                PasswordHash = PasswordHelper.HashPassword(newAdmin.Password),
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                DateOfBirth = null,
                IsActive = true
            };

            // Thêm newUser vào User
            await _userRepository.AddAsync(user);
            // Lưu thay đổi vào database
            await _dbu.SaveChangesAsync(); // Save to generate user.Id
            // Thêm Role vào bảng UserRoles
            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = newAdmin.GetRoleId()
            };

            // Thêm bảng tham chiếu
            user.UserRoles.Add(userRole);
            await _dbu.SaveChangesAsync(); // Save again to save UserRole

            return new RegisterResponseDTO
            {
                IsSuccess = true,
                Message = MessageRegister.REGISTER_SUCCESS,
                FullName = user.FullName,
                Email = user.Email
            };
        }
        catch (Exception error)
        {
            Console.Write($@"n{error.Message}");
            return new RegisterResponseDTO
            {
                IsSuccess = false,
                Message = MessageRegister.REGISTER_FAIL
            };
        }
    }

    public async Task<RegisterResponseDTO> RegisterCustomer(RegisterCustomerDTO newCustomer)
    {
        try
        {
            var checkCustomer = await _userRepository.SingleOrDefaultAsync(customer => customer.Email == newCustomer.Email || customer.UserName == newCustomer.Username);
            if (checkCustomer != null)
            {
                return new RegisterResponseDTO
                {
                    IsSuccess = false,
                    Message = checkCustomer.UserName == newCustomer.Username ? MessageRegister.USERNAME_EXIST : MessageRegister.EMAIL_EXIST
                };
            }

            var user = new User
            {
                UserName = newCustomer.Username,
                FullName = newCustomer.FullName,
                Email = newCustomer.Email,
                PhoneNumber = newCustomer.PhoneNumber,
                PasswordHash = PasswordHelper.HashPassword(newCustomer.Password),
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                DateOfBirth = null,
                IsActive = true
            };
            // Thêm newUser vào User
            await _userRepository.AddAsync(user);
            await _dbu.SaveChangesAsync(); // Save to generate user.Id

            // Thêm Role vào bảng UserRoles
            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = newCustomer.GetRoleId()
            };

            // Thêm bảng tham chiếu
            user.UserRoles.Add(userRole);
            // Lưu thay đổi vào database
            await _dbu.SaveChangesAsync();

            return new RegisterResponseDTO
            {
                IsSuccess = true,
                Message = MessageRegister.REGISTER_SUCCESS,
                FullName = user.FullName,
                Email = user.Email
            };
        }
        catch (Exception error)
        {
            return new RegisterResponseDTO
            {
                IsSuccess = false,
                Message = MessageRegister.REGISTER_FAIL
            };
        }
    }

    public async Task<LoginResponseDTO> LoginUser(LoginUserDTO userLogin)
    {
        try
        {
            var user = await _userRepository.SingleOrDefaultAsync(u => u.UserName == userLogin.UsernameOrEmail || u.Email == userLogin.UsernameOrEmail);
            if (user == null)
            {
                return new LoginResponseDTO { Message = MessageLogin.USER_NOT_FOUND };
            }

            // Kiểm tra mật khẩu
            if (!PasswordHelper.VerifyPassword(userLogin.Password, user.PasswordHash))
            {
                return new LoginResponseDTO { Message = MessageLogin.PASSWORD_INCORRECT };
            }
            // Kiểm tra tài khoản có bị vô hiệu hóa không
            var isStaff = user.UserRoles.Any(ur => ur.Role.Name == "Staff");
            if (isStaff)
            {
                var staffInfo = await _context.Staffs.FirstOrDefaultAsync(s => s.UserId == user.Id);
                if (staffInfo != null && (staffInfo.IsActive == false || staffInfo.IsDeleted == true))
                {
                    return new LoginResponseDTO { Message = "Tài khoản nhân viên đã bị vô hiệu hóa." };
                }
            }
            // Tạo token JWT
            var token = _jwtAuthService.GenerateToken(user);
            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
            return new LoginResponseDTO
            {
                AccessToken = token,
                FullName = user.FullName,
                AvatarUrl = user.AvatarUrl,
                Roles = roles,
                Message = MessageLogin.LOGIN_SUCCESS
            };
        }
        catch (Exception error)
        {
            return new LoginResponseDTO { Message = MessageLogin.ERROR_IN_SERVER };
        }
    }

    public async Task<bool> ApproveUpgradeToOwnerAsync(int requestId, int adminId)
    {
        var request = await _upgradeRequestRepository
        .SingleOrDefaultAsync(r => r.Id == requestId && r.Status == "Pending");
        if (request == null)
            return false;

        var user = await _userRepository.SingleOrDefaultAsync(u => u.Id == request.UserId);
        if (user == null)
            return false;

        // Kiểm tra xem user đã có role Owner chưa
        var hasOwnerRole = await _userRoleRepository
            .AnyAsync(ur => ur.UserId == user.Id && ur.RoleId == RoleTypeConstDTO.Owner);

        if (!hasOwnerRole)
        {
            // Thêm role Owner cho user
            var newUserRole = new UserRole
            {
                UserId = user.Id,
                RoleId = RoleTypeConstDTO.Owner
            };
            _context.UserRoles.Add(newUserRole);

            // Cập nhật trạng thái yêu cầu
            request.Status = "Approved";
            request.ApprovedAt = DateTime.Now;
            request.ApprovedBy = adminId;

            _upgradeRequestRepository.UpdateAsync(request);


            await _dbu.SaveChangesAsync();
            return true;
        }

        return false;
    }

    // public async Task<RegisterResponseDTO> RegisterStaff(RegisterStaffDTO newStaff, int ownerId)
    // {

    // }
    public async Task<UserProfileDTO> GetUserProfileAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId); // Hoặc _context.Users.Find(userId)
        if (user == null) return null;

        // Map Entity -> DTO
        return new UserProfileDTO
        {
            Id = user.Id,
            Username = user.UserName,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address,
            AvatarUrl = user.AvatarUrl,
            DateOfBirth = user.DateOfBirth,

        };
    }

    // Hàm update (để làm tính năng Edit sau này)
    public async Task<bool> UpdateUserProfileAsync(int userId, UserProfileDTO updateReq)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        user.FullName = updateReq.FullName;
        user.PhoneNumber = updateReq.PhoneNumber;
        user.Address = updateReq.Address;
        user.DateOfBirth = updateReq.DateOfBirth;
        // user.AvatarUrl = ... (Xử lý upload ảnh riêng)

        await _dbu.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateAvatarUrlAsync(int userId, string newAvatarUrl)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        user.AvatarUrl = newAvatarUrl;
        user.UpdatedAt = DateTime.Now;

        await _dbu.SaveChangesAsync();
        return true;
    }
}