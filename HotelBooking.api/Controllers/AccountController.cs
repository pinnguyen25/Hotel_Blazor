using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace HotelBooking.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IFileUploadService _uploadService;
        public AccountController(IUserService userService, IFileUploadService uploadService)
        {
            _userService = userService;
            _uploadService = uploadService;
        }

        [HttpGet("get-user-by-id")]
        public async Task<ActionResult> GetUserByIdAsync(int userId)
        {
            var user = await _userService.GetByIdAsync(userId);
            if (user == null) return NotFound("User not found.");
            return Ok(user);
        }

        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdminAsync([FromBody] RegisterAdminDTO newAdmin)
        {
            var res = await _userService.RegisterAdmin(newAdmin);
            if (res.IsSuccess)
            {
                return Ok(res);
            }
            else
            {
                return BadRequest(res.Message ?? MessageRegister.REGISTER_FAIL);
            }
        }

        [HttpPost("register-customer")]
        public async Task<IActionResult> RegisterCustomerAsync([FromBody] RegisterCustomerDTO newCustomer)
        {
            var res = await _userService.RegisterCustomer(newCustomer);
            if (res.IsSuccess)
            {
                return Ok(res);
            }
            else
            {
                return BadRequest(res.Message ?? MessageRegister.REGISTER_FAIL);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult> LoginUserAsync([FromBody] LoginUserDTO userLogin)
        {
            var res = await _userService.LoginUser(userLogin);

            if (res == null)
            {
                return StatusCode(500, MessageLogin.ERROR_IN_SERVER);
            }

            if (res.Message == MessageLogin.USER_NOT_FOUND)
            {
                return NotFound(res.Message);
            }
            else if (res.Message == MessageLogin.PASSWORD_INCORRECT)
            {
                return BadRequest(res.Message);
            }
            else if (res.Message == MessageLogin.ERROR_IN_SERVER)
            {
                return StatusCode(500, res.Message);
            }
            else
            {
                return Ok(res); // lúc này res chứa cả Token, UserInfo, Roles, Message
            }
        }

        // [HttpPost("upgrade-request")]
        // [Authorize(Roles = "Customer")]
        // public async Task<ActionResult> RequestUpgradeAsync()
        // {
        //     var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        //     if (claim == null || string.IsNullOrEmpty(claim.Value))
        //     {
        //         return BadRequest("User identifier claim is missing.");
        //     }

        //     var userId = int.Parse(claim.Value);
        //     var success = await _userService.RequestUpgradeToOwnerAsync(userId);
        //     if (!success) return BadRequest("Cannot process upgrade request.");
        //     else
        //         return Ok("Sent upgrade request successfully. Please wait for admin approval.");
        // }

        [HttpPost("upgrade-approve")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ApproveUpgradeAsync(int requestId)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null || string.IsNullOrEmpty(claim.Value))
            {
                return BadRequest("User identifier claim is missing.");
            }
            var adminId = int.Parse(claim.Value);
            var success = await _userService.ApproveUpgradeToOwnerAsync(requestId, adminId);
            if (!success) return BadRequest("Cannot approve upgrade request.");
            else
                return Ok("Approved upgrade request successfully.");
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetMyProfile()
        {
            // Lấy UserId từ Token (Claims)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); // hoặc "id" tùy cách bạn lưu token
            if (userIdClaim == null) return Unauthorized("Invalid Token");

            int userId = int.Parse(userIdClaim.Value);

            var profile = await _userService.GetUserProfileAsync(userId);
            if (profile == null) return NotFound("Không tìm thấy thông tin người dùng.");

            return Ok(profile);
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UserProfileDTO req)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var result = await _userService.UpdateUserProfileAsync(userId, req);
            if (result) return Ok("Profile updated successfully");
            return BadRequest("Update failed");
        }

        [HttpPost("avatar")]
        [Authorize]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Vui lòng chọn ảnh để upload.");

            var userId = GetCurrentUserId();

            // 1. Upload ảnh lên Cloudinary (thông qua UploadService)
            // Folder: users/{userId}/avatar
            string folderName = $"users/{userId}/avatar";
            
            // Gọi hàm SaveImageAsync (Bạn đã define trong IFileUploadService)
            // publicIdHint: null để Cloudinary tự sinh, hoặc set "avatar" để ghi đè (tùy logic bạn)
            string? avatarUrl = await _uploadService.SaveImageAsync(file, folderName);

            if (string.IsNullOrEmpty(avatarUrl))
                return StatusCode(500, "Lỗi khi upload ảnh lên server lưu trữ.");

            // 2. Cập nhật URL vào DB (thông qua UserService)
            var updateResult = await _userService.UpdateAvatarUrlAsync(userId, avatarUrl);

            if (!updateResult)
                return BadRequest("Lưu thông tin ảnh vào database thất bại.");

            // 3. Trả về URL để Frontend hiển thị ngay
            return Ok(avatarUrl); 
        }

        private int GetCurrentUserId()
        {
            // ClaimTypes.NameIdentifier thường được map với "sub" hoặc "id" trong JWT
            var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("id");
            
            if (claim == null || string.IsNullOrEmpty(claim.Value))
            {
                // Throw exception để middleware xử lý trả về 401, hoặc return 0 rồi check ở trên
                throw new UnauthorizedAccessException("Token không hợp lệ hoặc thiếu thông tin định danh.");
            }

            if (int.TryParse(claim.Value, out int userId))
            {
                return userId;
            }
            
            throw new UnauthorizedAccessException("ID người dùng trong Token không hợp lệ.");
        }
    }
    

}