using System.Text.Json;
using HotelBooking.infrastructure.Models;
using Microsoft.EntityFrameworkCore;

public interface IUpgradeRequestService
{
    public Task<UserForUpgradeDTO?> GetUserForUpgradeAsync(int userId);
    public Task<bool> CreateRequestAsync(int userId, string address, string taxCode);
    public Task<IEnumerable<UpgradeRequestDTO>> GetAllRequestAsync(string? status = null);
    public Task<UpgradeRequestDTO> GetByRequestIdAsync(int requestId);
    public Task<bool> ApproveRequestAsync(int requestId, int adminId);
    public Task<bool> RejectRequestAsync(int requestId, int adminId);

    // duyệt hotel
    public Task<AdminDashboardStatsDTO> GetHotelStatsAsync();
    public Task<bool> ApproveHotelAsync(int hotelId, int adminId);
    public Task<bool> RejectHotelAsync(int hotelId, int adminId, string reason);
    public Task<IEnumerable<HotelApprovalSummaryDTO>> GetPendingHotelsAsync();
    public Task<HotelDraftFullDTO> GetHotelDetailForReviewAsync(int hotelId);

    public Task<ApiResponse<PagedResult<HotelUpdateRequestAdminDTO>>> GetHotelUpdateRequestsAsync(string? status, int pageIndex, int pageSize);
    
    public Task<ApiResponse<bool>> ProcessHotelUpdateAsync(int requestId, int adminId, bool isApproved, string? reason);
}

public class UpgradeRequestService : IUpgradeRequestService
{
    HotelBookingContext _context;
    private readonly IUpgradeRequestRepository _upgradeRequestRepo;
    private readonly IUserRepository _userRepo;
    private readonly IUserRoleRepository _userRoleRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;

    public UpgradeRequestService(HotelBookingContext context, IEmailService emailService, IUpgradeRequestRepository upgradeRequestRepo, IUserRepository userRepo, IUserRoleRepository userRoleRepo, IUnitOfWork unitOfWork)
    {
        _context = context;
        _upgradeRequestRepo = upgradeRequestRepo;
        _userRepo = userRepo;
        _userRoleRepo = userRoleRepo;
        _unitOfWork = unitOfWork;

        _emailService = emailService;
    }

    public async Task<UserForUpgradeDTO?> GetUserForUpgradeAsync(int userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) return null;

        return new UserForUpgradeDTO
        {
            UserId = user.Id,
            UserName = user.UserName,
            FullName = user.FullName ?? "",
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };
    }

    public async Task<bool> CreateRequestAsync(int userId, string address, string taxCode)
    {
        if (string.IsNullOrWhiteSpace(address) || string.IsNullOrWhiteSpace(taxCode)) return false;

        // Check if user exists
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null)
        {
            return false; // User does not exist
        }

        // Check role qua UserRoles
        var hasCustomerRole = await _userRoleRepo
            .AnyAsync(ur => ur.UserId == userId && ur.RoleId == RoleTypeConstDTO.Customer);

        if (!hasCustomerRole)
            return false;

        // Check if there's already a pending request for this user
        var existingRequests = await _upgradeRequestRepo.GetPendingByIdAsync(userId);
        if (existingRequests.Any())
        {
            return false; // There's already a pending request
        }

        // Create new upgrade request
        var request = new UpgradeRequest
        {
            UserId = userId,
            Address = address,
            TaxCode = taxCode,
            Status = "Pending",
            RequestedAt = DateTime.Now
        };

        await _upgradeRequestRepo.AddAsync(request);
        return await _unitOfWork.SaveChangesAsync() > 0;
    }

    public async Task<IEnumerable<UpgradeRequestDTO>> GetAllRequestAsync(string? status = null)

    {
        var requests = await _upgradeRequestRepo.GetAllAsync();

        if (!string.IsNullOrEmpty(status))
        {
            requests = requests.Where(r => r.Status == status);
        }
        var results = new List<UpgradeRequestDTO>();
        foreach (var req in requests)
        {
            var user = await _userRepo.GetByIdAsync(req.UserId);
            if (user != null)
            {
                results.Add(InfoDTO(req, user));
            }
        }
        return results.OrderByDescending(r => r.RequestedAt);

    }

    public async Task<UpgradeRequestDTO> GetByRequestIdAsync(int requestId)
    {
        var request = await _upgradeRequestRepo.GetByIdAsync(requestId);
        if (request == null) return null!;

        var user = await _userRepo.GetByIdAsync(request.UserId);
        if (user == null) return null!;

        return InfoDTO(request, user);
    }

    private UpgradeRequestDTO InfoDTO(UpgradeRequest req, User user)
    {
        return new UpgradeRequestDTO
        {
            RequestId = req.Id,
            UserId = user.Id,
            UserName = user.UserName,
            FullName = user.FullName ?? "",
            PhoneNumber = user.PhoneNumber ?? "",
            Address = req.Address ?? "",
            TaxCode = req.TaxCode ?? "",
            AvatarUrl = user.AvatarUrl,
            Status = req.Status ?? "Pending",
            RequestedAt = req.RequestedAt
        };
    }

    public async Task<bool> ApproveRequestAsync(int requestId, int adminId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var request = await _upgradeRequestRepo.GetByIdAsync(requestId);
            if (request == null || request.Status != "Pending") return false;

            var user = await _userRepo.GetByIdAsync(request.UserId);
            if (user == null) return false;

            // 1. Thêm Role Owner (Nếu chưa có)
            var hasOwnerRole = await _userRoleRepo.AnyAsync(ur => ur.UserId == user.Id && ur.RoleId == RoleTypeConstDTO.Owner);
            if (!hasOwnerRole)
            {
                var ownerRole = new UserRole { UserId = user.Id, RoleId = RoleTypeConstDTO.Owner };
                await _userRoleRepo.AddAsync(ownerRole); // Giả sử repo này chỉ Add vào context
            }

            // 2. Cập nhật Request
            request.Status = "Approved";
            request.ApprovedAt = DateTime.Now;
            request.ApprovedBy = adminId;
            await _upgradeRequestRepo.UpdateAsync(request);

            // 3. Cập nhật thông tin thuế/địa chỉ cho User (quan trọng cho việc xuất hóa đơn sau này)
            user.Address = request.Address;
            user.TaxCode = request.TaxCode;
            await _userRepo.UpdateAsync(user);

            // 4. Lưu tất cả
            await _unitOfWork.SaveChangesAsync();

            // Commit Transaction
            await transaction.CommitAsync();
            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return false;
        }
    }

    public async Task<bool> RejectRequestAsync(int requestId, int adminId)
    {
        var request = await _upgradeRequestRepo.GetByIdAsync(requestId);
        if (request == null || request.Status != "Pending")
        {
            return false; // Request not found or not pending
        }

        // Update request status to Rejected
        request.Status = "Rejected";
        request.ApprovedAt = DateTime.Now;
        request.ApprovedBy = adminId;
        await _upgradeRequestRepo.UpdateAsync(request);

        return await _unitOfWork.SaveChangesAsync() > 0;
    }

    #region Request hotel

    public async Task<AdminDashboardStatsDTO> GetHotelStatsAsync()
    {
        var stats = await _context.Hotels
            .GroupBy(h => h.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();

        return new AdminDashboardStatsDTO
        {
            PendingHotels = stats.FirstOrDefault(x => x.Status == "PendingVerification")?.Count ?? 0,
            ActiveHotels = stats.FirstOrDefault(x => x.Status == "Active")?.Count ?? 0,
            RejectedHotels = stats.FirstOrDefault(x => x.Status == "Rejected")?.Count ?? 0
        };
    }

    public async Task<IEnumerable<HotelApprovalSummaryDTO>> GetPendingHotelsAsync()
    {
        return await _context.Hotels
            .AsNoTracking()
            .Where(h => h.Status == "PendingVerification" && h.IsDeleted == false)
            .Include(h => h.Owner)
            .Include(h => h.City)
            .OrderByDescending(h => h.UpdatedAt) // Mới nhất lên đầu
            .Select(h => new HotelApprovalSummaryDTO
            {
                HotelId = h.Id,
                HotelName = h.Name,
                OwnerName = h.Owner.FullName!, // Lấy tên chủ
                CoverImageUrl = h.CoverImageUrl!,
                Address = h.Address,       // Địa chỉ chi tiết (số nhà, đường)
                CityName = h.City.Name,// Gộp địa chỉ cho gọn
                SubmittedAt = h.UpdatedAt ?? h.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<ApiResponse<PagedResult<HotelUpdateRequestAdminDTO>>> GetHotelUpdateRequestsAsync(string? status, int pageIndex, int pageSize)
    {
        try 
        {
            var query = _context.HotelUpdateRequests
                .AsNoTracking()
                .Include(r => r.Hotel).ThenInclude(h => h.Owner)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(r => r.Status == status);
            }
            else 
            {
                // Mặc định ưu tiên Pending
                query = query.OrderByDescending(r => r.Status == "Pending")
                             .ThenByDescending(r => r.CreatedAt);
            }

            var totalCount = await query.CountAsync();
            
            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Map sang DTO (Thực hiện ở Client side sau khi lấy list để tránh lỗi JSON trong LINQ to SQL)
            var dtos = items.Select(r => new HotelUpdateRequestAdminDTO
            {
                RequestId = r.Id,
                HotelId = r.HotelId,
                HotelName = r.Hotel.Name,
                OwnerName = r.Hotel.Owner?.FullName ?? "N/A",
                RequestedAt = r.CreatedAt ?? DateTime.MinValue,
                Status = r.Status,
                
                // Deserialize dữ liệu mới owner muốn sửa
                NewData = JsonSerializer.Deserialize<HotelCreateOrUpdateDTO>(r.UpdateContentJson)!,
                
                // Map dữ liệu hiện tại trong DB để Admin so sánh
                CurrentData = new HotelCreateOrUpdateDTO
                {
                    Name = r.Hotel.Name,
                    Address = r.Hotel.Address,
                    CityId = r.Hotel.CityId,
                    Description = r.Hotel.Description,
                    ContactName = r.Hotel.ContactName,
                    ContactPhone = r.Hotel.ContactPhone,
                    ContactEmail = r.Hotel.ContactEmail,
                    AccommodationTypeId = r.Hotel.AccommodationTypeId,
                    ChainId = r.Hotel.ChainId
                }
            }).ToList();

            return ApiResponseHelper.Ok(new PagedResult<HotelUpdateRequestAdminDTO>
            {
                Items = dtos,
                TotalRecords = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            });
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ServerError<PagedResult<HotelUpdateRequestAdminDTO>>(ex.Message);
        }
    }

    public async Task<ApiResponse<bool>> ProcessHotelUpdateAsync(int requestId, int adminId, bool isApproved, string? reason)
    {
        // Dùng transaction vì có sửa bảng Hotel
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var request = await _context.HotelUpdateRequests
                .Include(r => r.Hotel) // Include để update luôn Hotel nếu duyệt
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null || request.Status != "Pending") 
                return ApiResponseHelper.NotFound<bool>("Yêu cầu không tồn tại hoặc đã được xử lý.");

            request.ProcessedBy = adminId;
            request.ProcessedAt = DateTime.Now;
            request.AdminNote = reason;

            if (isApproved)
            {
                request.Status = "Approved";

                // === ÁP DỤNG DỮ LIỆU TỪ JSON VÀO HOTEL ===
                var newData = JsonSerializer.Deserialize<HotelCreateOrUpdateDTO>(request.UpdateContentJson);
                if (newData != null)
                {
                    var hotel = request.Hotel;
                    // Map các trường cho phép sửa
                    hotel.Name = newData.Name;
                    hotel.Address = newData.Address;
                    hotel.CityId = newData.CityId;
                    hotel.Description = newData.Description;
                    hotel.ContactName = newData.ContactName;
                    hotel.ContactPhone = newData.ContactPhone;
                    hotel.ContactEmail = newData.ContactEmail;
                    hotel.AccommodationTypeId = newData.AccommodationTypeId;
                    hotel.ChainId = newData.ChainId;
                    
                    hotel.UpdatedAt = DateTime.Now;
                }
            }
            else
            {
                request.Status = "Rejected";
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            
            // TODO: Gửi email thông báo cho Owner (nếu cần)

            return ApiResponseHelper.Ok(true, isApproved ? "Đã duyệt cập nhật thông tin." : "Đã từ chối cập nhật.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return ApiResponseHelper.ServerError<bool>(ex.Message);
        }
    }


    public async Task<HotelDraftFullDTO> GetHotelDetailForReviewAsync(int hotelId)
    {
        // Lấy Hotel và Owner
        var hotel = await _context.Hotels
            .Include(h => h.Owner)
            .FirstOrDefaultAsync(h => h.Id == hotelId);

        if (hotel == null) return null!;

        // --- 1. Basic Info ---
        var basic = new HotelResponseDTO
        {
            HotelId = hotel.Id,
            Name = hotel.Name,
            Address = hotel.Address,
            CityId = hotel.CityId,
            Description = hotel.Description,
            ContactName = hotel.ContactName,
            ContactPhone = hotel.ContactPhone,
            ContactEmail = hotel.ContactEmail,
            AccommodationTypeId = hotel.AccommodationTypeId ?? 0,
            ChainId = hotel.ChainId ?? 0
        };

        // --- 2. Images ---
        var images = new HotelImagesResponseDTO
        {
            CoverImageUrl = hotel.CoverImageUrl,
            GalleryImageUrls = await _context.HotelImages
                .Where(hi => hi.HotelId == hotelId && hi.IsDeleted == false)
                .OrderBy(hi => hi.SortOrder)
                .Select(hi => hi.ImageUrl)
                .ToListAsync()
        };

        // --- 3. Amenities & Policies ---
        var amenityIds = await _context.HotelAmenities
            .Where(ha => ha.HotelId == hotelId)
            .Select(ha => ha.AmenityId)
            .ToListAsync();

        // Lấy Policy Hệ Thống (Có ID > 0)
        var systemPolicies = await _context.HotelPolicies
            .Where(hp => hp.HotelId == hotelId && hp.PolicyId > 0)
            .Include(hp => hp.Policy)
            .Select(hp => new PolicyDTO
            {
                Id = hp.PolicyId,
                Name = hp.Policy.Name,
                Description = hp.Policy.Description,
                PolicyTypeId = hp.Policy.PolicyTypeId,
                IsSystemPolicy = hp.Policy.IsSystemPolicy,
            })
            .ToListAsync();

        // Lấy Custom Policy (PolicyId = 0, lưu Json trong Additional)
        var customPolicyJsons = await _context.HotelPolicies
            .AsNoTracking()
            .Where(hp => hp.HotelId == hotelId && hp.PolicyId == 0 && hp.Additional != null)
            .Select(hp => hp.Additional)
            .ToListAsync();

        var customPolicies = customPolicyJsons
            .Select(json => JsonSerializer.Deserialize<OwnerCustomPolicyDTO>(json!)!)
            .ToList();

        // --- 4. Services ---
        var services = await _context.HotelServiceConfigs
            .Where(hs => hs.HotelId == hotelId)
            .Include(hs => hs.Service)
            .Select(hs => new OwnerHotelServiceDTO
            {
                Id = hs.Id,
                ServiceId = hs.ServiceId,
                ServiceName = hs.Service.Name,
                Description = hs.Service.Description,
                Price = hs.Price,
                Unit = hs.Unit,
                IsActive = hs.IsActive ?? true
            })
            .ToListAsync();

        // --- ROOM TYPES ---
        // Admin cần xem phòng để duyệt giá
        var roomTypes = await _context.RoomTypes
            .AsNoTracking()
            .AsSplitQuery()
            .Where(rt => rt.HotelId == hotelId && rt.IsDeleted == false)
            .Include(rt => rt.RoomImages)
            // Include đầy đủ để hiển thị chi tiết cho Admin
            .Include(rt => rt.RoomBedTypes).ThenInclude(rb => rb.BedType)
            .Include(rt => rt.RoomViewTypes).ThenInclude(rv => rv.ViewType)
            .Include(rt => rt.RoomAmenities).ThenInclude(ra => ra.Amenity)
            .Include(rt => rt.RoomTypeServices).ThenInclude(rts => rts.Service)
            .OrderBy(rt => rt.SortOrder)
            .Select(rt => new RoomTypeForOwnerDTO
            {
                Id = rt.Id,
                Name = rt.Name,
                PricePerNight = rt.PricePerNight,
                Quantity = rt.Quantity,
                DefaultImageUrl = rt.DefaultImageUrl,
                AdultCapacity = rt.AdultCapacity,
                ChildCapacity = rt.ChildCapacity ?? 0,
                Area = rt.Area,
                Description = rt.Description,
                IsActive = rt.IsActive ?? true,
                // Map Amenities (Để Admin check tiện ích phòng)
                Amenities = rt.RoomAmenities.Select(ra => new AmenityDTO
                {
                    Id = ra.AmenityId,
                    Name = ra.Amenity.Name
                }).ToList(),

                // Map Beds
                Beds = rt.RoomBedTypes.Select(rb => new RoomBedTypeDTO
                {
                    BedTypeId = rb.BedTypeId,
                    BedTypeName = rb.BedType.Name,
                    Quantity = rb.Quantity
                }).ToList(),

                // Map Views
                Views = rt.RoomViewTypes.Select(rv => new RoomViewTypeDTO
                {
                    ViewTypeId = rv.ViewTypeId,
                    ViewTypeName = rv.ViewType.Name
                }).ToList(),

                // Admin cần thấy chi tiết (Quantity, Note) chứ không chỉ tên như User
                RoomTypeServices = rt.RoomTypeServices.Select(rts => new RoomTypeServiceDTO
                {
                    ServiceId = rts.ServiceId,
                    ServiceName = rts.Service.Name,
                    Quantity = rts.Quantity ?? 1,
                    Note = rts.Note
                }).ToList()
            })
            .ToListAsync();

        return new HotelDraftFullDTO
        {
            BasicInfo = basic,
            Images = images,
            SelectedAmenityIds = amenityIds,
            SelectedPolicyIds = systemPolicies
                .Where(p => p.Id.HasValue)
                .Select(p => p.Id!.Value)
                .ToList(),
            SystemPolicies = systemPolicies,
            CustomPolicies = customPolicies,
            HotelServices = services,
            RoomTypes = roomTypes
        };
    }

    public async Task<bool> ApproveHotelAsync(int hotelId, int adminId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 1. Load Hotel + Owner Info (để lấy email gửi)
            var hotel = await _context.Hotels
                .Include(h => h.Owner) // Quan trọng: Include Owner để lấy Email
                .FirstOrDefaultAsync(h => h.Id == hotelId);

            if (hotel == null || hotel.Status != "PendingVerification")
                return false;

            // 2. Cập nhật trạng thái
            hotel.Status = "Active";
            hotel.IsVerified = true;
            hotel.UpdatedAt = DateTime.Now;
            // hotel.ApprovedBy = adminId; 

            await _context.SaveChangesAsync();
            await transaction.CommitAsync(); // Commit xong mới gửi mail


            // Nên try-catch riêng phần email để tránh việc DB xong mà Email lỗi lại rollback DB (không đáng)
            try
            {
                // 1. Chuẩn bị dữ liệu
                string logoUrl = "https://res.cloudinary.com/dmvsguisc/image/upload/v1766036745/25c3fb5f-2c13-48b8-bea3-0949e0017f27.png"; // Thay bằng link logo thật của bạn
                string dashboardUrl = "https://localhost:5001/owner/hotels/9005/dashboard"; // Thay bằng link thật của bạn
                string currentYear = DateTime.Now.Year.ToString();

                // 2. Tạo nội dung HTML Email
                var emailBody = $@"
<!DOCTYPE html>
<html>
<head>
<style>
    /* Reset Styles */
    body, table, td, a {{ -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; }}
    table, td {{ mso-table-lspace: 0pt; mso-table-rspace: 0pt; }}
    img {{ -ms-interpolation-mode: bicubic; border: 0; height: auto; line-height: 100%; outline: none; text-decoration: none; display: block; }}
    body {{ height: 100% !important; margin: 0 !important; padding: 0 !important; width: 100% !important; background-color: #f4f6f8; font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif; }}
    
    /* Custom Styles */
    .ticket-container {{ max-width: 600px; margin: 40px auto; background-color: #ffffff; border-radius: 16px; overflow: hidden; box-shadow: 0 10px 30px rgba(84, 169, 255, 0.15); }}
    .ticket-header {{ background: linear-gradient(135deg, #54a9ff 0%, #85c4ff 100%); padding: 30px; text-align: center; position: relative; border-bottom: 4px dashed rgba(255, 255, 255, 0.4);}}
    .ticket-body {{ padding: 30px; }}
    .ticket-footer {{ background-color: #f8faff; padding: 20px; text-align: center; border-top: 2px dashed #dceeff; }}
    
    .info-label {{ color: #7f8c8d; font-size: 12px; text-transform: uppercase; font-weight: 600; letter-spacing: 1px; margin-bottom: 5px; }}
    .info-value {{ color: #2c3e50; font-size: 16px; font-weight: 700; margin-bottom: 20px; }}
    .status-active {{ color: #28a745; display: inline-block; padding: 5px 12px; background-color: #e6f9ed; border-radius: 20px; font-size: 14px; font-weight: 600; }}
    .cta-button {{ display: inline-block; padding: 15px 40px; background-color: #54a9ff; color: #ffffff !important; text-decoration: none; border-radius: 50px; font-weight: bold; font-size: 16px; box-shadow: 0 4px 15px rgba(84, 169, 255, 0.3); transition: all 0.3s ease; }}
</style>
</head>
<body>
    <table border='0' cellpadding='0' cellspacing='0' width='100%'>
        <tr>
            <td align='center' style='padding: 20px;'>
                
                <div class='ticket-container ticket-card'>
                    
                    <div class='ticket-header'>
                        <img src='{logoUrl}' alt='ChillZone Logo' width='150' style='margin: 0 auto 20px auto;' />
                        
                        <h1 style='color: #ffffff; font-size: 26px; margin: 0;'>Chúc mừng, {hotel.Owner.FullName}!</h1>
                        <p style='color: #e6f2ff; font-size: 16px; margin: 10px 0 0 0;'>Khách sạn của bạn đã chính thức hoạt động.</p>
                        
                    </div>

                    <div class='ticket-body'>
                        
                        <table border='0' cellpadding='0' cellspacing='0' width='100%'>
                            <tr>
                                <td width='50%' valign='top'>
                                    <div class='info-label'>Tên khách sạn</div>
                                    <div class='info-value'>{hotel.Name}</div>
                                </td>
                                <td width='50%' valign='top' align='right'>
                                    <div class='info-label'>Mã khách sạn (ID)</div>
                                    <div class='info-value'>#{hotel.Id}</div>
                                </td>
                            </tr>
                            <tr>
                                <td colspan='2' valign='top'>
                                    <div class='info-label'>Địa chỉ</div>
                                    <div class='info-value'>{hotel.Address}</div>
                                </td>
                            </tr>
                            <tr>
                                <td colspan='2' valign='top'>
                                    <div class='info-label'>Trạng thái</div>
                                    <div><span class='status-active'>● Đang hoạt động</span></div>
                                </td>
                            </tr>
                        </table>
                        
                        <div style='border-bottom: 2px dashed #dceeff; margin: 30px 0;'></div>

                        <div style='text-align: center;'>
                            <p style='color: #7f8c8d; font-size: 15px; margin-bottom: 25px;'>Bạn đã sẵn sàng đón những vị khách đầu tiên chưa?</p>
                            <a href='{dashboardUrl}' target='_blank' class='cta-button'>
                                Truy cập Trang quản lý &rarr;
                            </a>
                        </div>
                    </div>

                    <div class='ticket-footer'>
                        <p style='color: #b0b0b0; font-size: 12px; margin: 0;'>
                            &copy; {currentYear} ChillZone. All rights reserved.<br>
                            Email này được gửi tự động, vui lòng không trả lời.
                        </p>
                    </div>
                </div>
            </td>
        </tr>
    </table>
</body>
</html>";

                await _emailService.SendEmailAsync(hotel.Owner.Email, $"[ChillZone] 🎉 Chúc mừng! {hotel.Name} đã được duyệt", emailBody);
            }
            catch (Exception emailEx)
            {
                // Chỉ log lỗi email, không throw exception để làm fail transaction
                Console.WriteLine($"Gửi mail thất bại: {emailEx.Message}");
            }

            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return false;
        }
    }

    public async Task<bool> RejectHotelAsync(int hotelId, int adminId, string reason)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var hotel = await _context.Hotels
                .Include(h => h.Owner)
                .FirstOrDefaultAsync(h => h.Id == hotelId);

            if (hotel == null || hotel.Status != "PendingVerification")
                return false;

            // 1. Cập nhật trạng thái
            hotel.Status = "Rejected";
            hotel.IsVerified = false;
            hotel.UpdatedAt = DateTime.Now;
            // Lưu lý do từ chối (giả sử bạn có cột Note hoặc RejectionReason)
            var additionalData = new Dictionary<string, object>();
            // Ghi đè hoặc thêm vào dữ liệu hiện có
            additionalData["RejectionReason"] = reason;
            if (!string.IsNullOrEmpty(hotel.Additional))
            {
                try
                {
                    additionalData = JsonSerializer.Deserialize<Dictionary<string, object>>(hotel.Additional)
                                     ?? new Dictionary<string, object>();
                }
                catch { /* Nếu dữ liệu cũ không phải JSON thì bỏ qua hoặc xử lý riêng */ }
            }
            // Cập nhật lý do từ chối và thông tin admin
            additionalData["RejectionReason"] = reason;
            additionalData["RejectedBy"] = adminId;
            additionalData["RejectedAt"] = DateTime.Now;
            // Chuyển đổi lại thành JSON để lưu
            hotel.Additional = JsonSerializer.Serialize(additionalData);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            // 2. Gửi Email thông báo từ chối kèm lý do
            try
            {
                // Thay link logo và dashboard của bạn
                string logoUrl = "https://res.cloudinary.com/dmvsguisc/image/upload/v1766039150/zone_xlcibc.png";
                string dashboardUrl = "https://localhost:5001/owner/dashboard";
                string currentYear = DateTime.Now.Year.ToString();

                // Icon cảnh báo (dấu chấm than)
                string alertIconUrl = "https://cdn-icons-png.flaticon.com/512/179/179386.png";

                var emailBody = $@"
<!DOCTYPE html>
<html>
<head>
<style>
    body, table, td, a {{ -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; }}
    table, td {{ mso-table-lspace: 0pt; mso-table-rspace: 0pt; }}
    img {{ -ms-interpolation-mode: bicubic; border: 0; display: block; outline: none; text-decoration: none; }}
    body {{ height: 100% !important; margin: 0 !important; padding: 0 !important; width: 100% !important; background-color: #f4f6f8; font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif; }}
    
    .card-container {{ max-width: 600px; margin: 40px auto; background-color: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 20px rgba(0,0,0,0.08); border-top: 5px solid #ff4d4f; }}
    .header {{ padding: 40px 30px 20px 30px; text-align: center; }}
    .content {{ padding: 0 40px 40px 40px; }}
    .reason-box {{ background-color: #fff1f0; border: 1px solid #ffccc7; border-radius: 8px; padding: 20px; margin: 20px 0; text-align: left; }}
    .reason-title {{ color: #cf1322; font-weight: bold; font-size: 14px; text-transform: uppercase; margin-bottom: 5px; }}
    .reason-text {{ color: #262626; font-size: 15px; line-height: 1.5; }}
    .cta-button {{ display: inline-block; padding: 14px 30px; background-color: #2c3e50; color: #ffffff !important; text-decoration: none; border-radius: 6px; font-weight: bold; font-size: 15px; transition: all 0.3s ease; }}
    .footer {{ background-color: #f8f9fa; padding: 20px; text-align: center; border-top: 1px solid #eee; }}
</style>
</head>
<body>
    <table border='0' cellpadding='0' cellspacing='0' width='100%'>
        <tr>
            <td align='center' style='padding: 20px;'>
                <div class='card-container'>
                    
                    <div class='header'>
                        <img src='{logoUrl}' alt='ChillZone' width='100' style='margin: 0 auto 20px auto;' />
                        <img src='{alertIconUrl}' alt='Alert' width='60' style='margin: 0 auto 15px auto; opacity: 0.8;' />
                        <h2 style='color: #2c3e50; margin: 0 0 10px 0; font-size: 22px;'>Cần bổ sung thông tin</h2>
                        <p style='color: #7f8c8d; font-size: 15px; margin: 0; line-height: 1.5;'>
                            Chào {hotel.Owner.FullName},<br/>
                            Yêu cầu đăng ký khách sạn <b>{hotel.Name}</b> chưa được thông qua.
                        </p>
                    </div>

                    <div class='content'>
                        <div class='reason-box'>
                            <div class='reason-title'>🔴 Lý do từ chối:</div>
                            <div class='reason-text'>{reason}</div>
                        </div>

                        <p style='color: #595959; font-size: 14px; text-align: center; margin-bottom: 25px;'>
                            Vui lòng kiểm tra lại thông tin và cập nhật theo yêu cầu trên để chúng tôi xét duyệt lại.
                        </p>

                        <div style='text-align: center;'>
                            <a href='{dashboardUrl}' target='_blank' class='cta-button'>
                                Chỉnh sửa & Gửi lại
                            </a>
                        </div>
                    </div>

                    <div class='footer'>
                        <p style='color: #b0b0b0; font-size: 12px; margin: 0;'>
                            Nếu bạn cần hỗ trợ, vui lòng liên hệ Admin.<br>
                            &copy; {DateTime.Now.Year} ChillZone.
                        </p>
                    </div>
                </div>
            </td>
        </tr>
    </table>
</body>
</html>";

                await _emailService.SendEmailAsync(hotel.Owner.Email, $"[ChillZone] ⚠️ Yêu cầu cần bổ sung thông tin: {hotel.Name}", emailBody);
            }
            catch (Exception emailEx)
            {
                Console.WriteLine($"Gửi mail thất bại: {emailEx.Message}");
            }

            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return false;
        }
    }
    #endregion

}