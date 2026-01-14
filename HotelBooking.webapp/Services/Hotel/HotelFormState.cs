// Services/HotelFormState.cs
using System.Net.Http.Headers;
using Blazored.LocalStorage;
// using HotelBooking.Client.Models;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Microsoft.JSInterop;
using HotelBooking.webapp.ViewModels.Hotel;
using System.Text.Json;
public sealed class HotelFormState
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly ISnackbar _snackbar;
    private const string STORAGE_KEY = HotelDraftSavedState.STORAGE_KEY;
    private System.Timers.Timer _saveTimer;
    public HotelFormState(IHttpClientFactory factory, ILocalStorageService localStorage, ISnackbar snackbar)
    {
        _httpClient = factory.CreateClient("HotelBookingAPI");
        _localStorage = localStorage;
        _snackbar = snackbar;

        // Setup timer auto-save (sau 1.5s ngừng gõ sẽ lưu xuống LocalStorage)
        _saveTimer = new System.Timers.Timer(1500);
        _saveTimer.Elapsed += async (sender, e) => await PersistAsync();
        _saveTimer.AutoReset = false;
    }

    // TRẠNG THÁI CHUNG
    public int DraftHotelId { get; private set; } // 0 = chưa tạo
    public bool IsDraftCreated => DraftHotelId > 0;

    // DỮ LIỆU TỪNG BƯỚC
    public HotelCreateOrUpdateVM BasicInfo { get; set; } = new();
    public HotelImagesVM Images { get; set; } = new();
    // Tiện ích & chính sách
    public HashSet<int> SelectedAmenityIds { get; private set; } = new();
    public Dictionary<int, int?> SelectedPolicyByType { get; private set; } = new(); // Key: PolicyTypeId, Value: PolicyId
    public List<string> CustomPolicies { get; private set; } = new();
    // Danh sách load từ API
    public List<CityVM> Cities { get; private set; } = new();
    public List<AmenityVM> AllAmenities { get; set; } = new();
    public List<PolicyTypeVM> PolicyGroups { get; set; } = new();
    public List<RoomTypeDetailVM> RoomTypes { get; private set; } = new();
    public List<AccommodationTypeVM> AccommodationTypes { get; private set; } = new();
    public List<OwnerHotelServiceVM> HotelServices { get; private set; } = new();
    public event Action? OnChange;

    public void Notify()
    {
        _saveTimer.Stop();
        _saveTimer.Start();
        OnChange?.Invoke();
    }

    // --- VALIDATION LOGIC (Tập trung tại đây) ---
    public bool BasicInfoIsValid =>
        !string.IsNullOrWhiteSpace(BasicInfo.Name) &&
        !string.IsNullOrWhiteSpace(BasicInfo.Address) &&
        BasicInfo.CityId > 0 &&
        !string.IsNullOrWhiteSpace(BasicInfo.ContactPhone);

    // Kiểm tra ảnh kỹ hơn: Có cover và ít nhất 1 ảnh gallery
    public bool ImagesIsValid =>
        !string.IsNullOrEmpty(Images.CoverImageUrl) &&
        Images.GalleryImageUrls != null &&
        Images.GalleryImageUrls.Any();


    // load city
    public async Task LoadCitiesAsync()
    {
        try
        {
            await SetAuthHeader();
            var response = await _httpClient.GetAsync("hotel/get-cityName");
            if (response.IsSuccessStatusCode)
            {
                var cities = await response.Content.ReadFromJsonAsync<List<CityVM>>();
                if (cities != null)
                {
                    Cities = cities;
                    BasicInfo.Cities = Cities; // Nếu VM có property này
                    Notify();
                }
            }
        }
        catch (Exception ex)
        {
            _snackbar.Add($"Lỗi tải thành phố: {ex.Message}", Severity.Error);
        }
    }

    public async Task LoadAmenitiesAsync()
    {
        if (AllAmenities.Any()) return; // đã load r thì k load lại
        try
        {
            await SetAuthHeader();
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<AmenityVM>>>("hotel/get-all-amenities"); // API của bạn
            AllAmenities = response?.Content ?? new();
            // Parse Additional để lấy Icon
            foreach (var a in AllAmenities)
            {
                if (!string.IsNullOrWhiteSpace(a.Additional))
                {
                    try
                    {
                        var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(a.Additional);
                        a.IconClass = dict?.GetValueOrDefault("IconClass") ?? "fa-bed";
                        a.IconColor = dict?.GetValueOrDefault("IconColor") ?? "#54a9ffff";
                    }
                    catch { }
                }
            }
            Notify();
        }
        catch (Exception ex)
        {
            _snackbar.Add($"Lỗi tải tiện ích: {ex.Message}", Severity.Error);
        }
    }

    public async Task LoadPoliciesAsync()
    {
        if (PolicyGroups.Any()) return; // Đã load 

        try
        {
            await SetAuthHeader();
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<PolicyTypeVM>>>("hotel/get-all-policy");
            PolicyGroups = response?.Content ?? new();
            Notify();
        }
        catch (Exception ex)
        {
            _snackbar.Add($"Lỗi tải chính sách: {ex.Message}", Severity.Error);
        }
    }

    public async Task LoadRoomTypesAsync()
    {
        if (DraftHotelId == 0) return;
        try
        {
            await SetAuthHeader();
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<RoomTypeDetailVM>>>($"hotel/owner/{DraftHotelId}/roomtypes");
            if (response?.StatusCode == StatusCodeResponse.Success)
            {
                RoomTypes = response.Content ?? new();
            }
            Notify();
        }
        catch (Exception ex)
        {
            _snackbar.Add("Lỗi tải loại phòng: " + ex.Message, Severity.Error);
        }
    }

    public async Task LoadAccommodationTypesAsync()
    {
        if (AccommodationTypes.Any()) return;
        try
        {
            await SetAuthHeader();
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<AccommodationTypeVM>>>("hotel/get-all-accommodations");
            AccommodationTypes = response?.Content ?? new();
        }
        catch (Exception ex)
        {
            _snackbar.Add($"Lỗi tải mô hình lưu trú: {ex.Message}", Severity.Error);
        }
    }

    // load hotelservice
    public async Task LoadHotelServicesAsync()
    {
        if (DraftHotelId <= 0)
        {
            HotelServices = new();
            return;
        }

        try
        {
            await SetAuthHeader();
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<OwnerHotelServiceVM>>>($"hotel/owner/get-owner-services/{DraftHotelId}");
            if (response?.StatusCode == StatusCodeResponse.Forbidden || response?.StatusCode == StatusCodeResponse.NotFound)
            {
                _snackbar.Add("Phiên làm việc bản nháp đã hết hạn hoặc không hợp lệ. Đang tạo mới...", Severity.Warning);
                await ClearDraftAsync(); // <--- Xóa rác trong LocalStorage ngay
                return;

            }
            HotelServices = response?.Content ?? new();
            Notify();
        }
        catch (Exception ex)
        {
            _snackbar.Add($"Lỗi tải dịch vụ: {ex.Message}", Severity.Error);
        }

    }

    // Load từ localStorage khi khởi động
    public async Task LoadFromStorageAsync()
    {
        var saved = await _localStorage.GetItemAsync<HotelDraftSavedState>(STORAGE_KEY);
        if (saved != null)
        {
            if (DateTime.UtcNow - saved.Timestamp > TimeSpan.FromMinutes(20))
            {
                await ClearDraftAsync();
                return;
            }

            DraftHotelId = saved.DraftHotelId;
            BasicInfo = saved.BasicInfo ?? new();
            Images = saved.Images ?? new();
            SelectedAmenityIds = new HashSet<int>(saved.SelectedAmenityIds);
            SelectedPolicyByType = saved.SelectedPolicyByType?.ToDictionary(x => x.Key, x => (int?)x.Value)
                                     ?? new Dictionary<int, int?>();
            CustomPolicies = saved.CustomPolicies?.ToList() ?? new List<string>();
            HotelServices = saved.HotelServices ?? new List<OwnerHotelServiceVM>();
            Notify();

            if (IsDraftCreated)
            {
                await SyncImagesFromServerAsync();
                await LoadRoomTypesAsync();
            }
        }
    }

    // Lưu vào localStorage mỗi khi thay đổi quan trọng
    private async Task PersistAsync()
    {
        try
        {
            var toSave = new HotelDraftSavedState
            {
                DraftHotelId = DraftHotelId,
                BasicInfo = BasicInfo,
                Images = Images,
                SelectedAmenityIds = SelectedAmenityIds.ToList(),
                SelectedPolicyByType = SelectedPolicyByType.Where(x => x.Value.HasValue).ToDictionary(x => x.Key, x => x.Value!.Value),
                CustomPolicies = CustomPolicies.ToList(),
                HotelServices = HotelServices,
                Timestamp = DateTime.UtcNow
            };
            await _localStorage.SetItemAsync(STORAGE_KEY, toSave);
        }
        catch (Exception ex)
        {
            _snackbar.Add("Persist Error: " + ex.Message);
        }
    }

    public void Dispose()
    {
        if (_saveTimer != null)
        {
            _saveTimer.Stop();
            _saveTimer.Dispose();
            _saveTimer = null;
        }
    }

    public async Task<bool> LoadDraftAsync(int hotelId)
    {
        try
        {
            if (PolicyGroups == null || !PolicyGroups.Any())
            {
                await LoadPoliciesAsync();
            }
            // 1. Gọi API lấy dữ liệu từ Server
            await SetAuthHeader();
            //  trả về full thông tin
            var resp = await _httpClient.GetFromJsonAsync<ApiResponse<HotelDraftVM>>($"hotel/owner/{hotelId}/draft");

            if (resp?.StatusCode != StatusCodeResponse.Success || resp.Content == null)
            {
                _snackbar.Add("Không tải được dữ liệu.", Severity.Error);
                await ClearDraftAsync();
                return false;
            }

            var data = resp.Content;

            // 2. Gán dữ liệu từ Server vào State (RAM)
            DraftHotelId = hotelId;

            // Map Basic Info
            BasicInfo = new HotelCreateOrUpdateVM
            {
                HotelId = hotelId,
                Name = data.BasicInfo.Name,
                Address = data.BasicInfo.Address,
                CityId = data.BasicInfo.CityId,
                Description = data.BasicInfo.Description,
                ContactPhone = data.BasicInfo.ContactPhone,
                ContactName = data.BasicInfo.ContactName,
                ContactEmail = data.BasicInfo.ContactEmail,
                AccommodationTypeId = data.BasicInfo.AccommodationTypeId,
                ChainId = data.BasicInfo.ChainId
            };

            // Map Images
            Images = new HotelImagesVM
            {
                CoverImageUrl = data.Images.CoverImageUrl,
                GalleryImageUrls = data.Images.GalleryImageUrls ?? new List<string>()
            };

            // Map Amenities
            if (data.SelectedAmenityIds != null && data.SelectedAmenityIds.Any())
            {
                SelectedAmenityIds = new HashSet<int>(data.SelectedAmenityIds);
            }
            else if (DraftHotelId == hotelId && SelectedAmenityIds.Any())
            {
                // Không làm gì cả, giữ nguyên SelectedAmenityIds đang có trong RAM 
                Console.WriteLine("Keep Local Amenities because Server is empty.");
            }
            // 3. Nếu cả Server rỗng và LocalStorage không khớp -> Reset về rỗng
            else
            {
                SelectedAmenityIds.Clear();
            }

            // Map Policies
            SelectedPolicyByType.Clear();
            if (data.SelectedPolicyIds != null)
            {
                foreach (var pid in data.SelectedPolicyIds)
                {
                    var group = PolicyGroups.FirstOrDefault(g => g.Policies!.Any(p => p.Id == pid));
                    if (group != null)
                    {
                        SelectedPolicyByType[group.Id] = pid;
                    }
                }
            }

            CustomPolicies = data.CustomPolicies?.Select(x => x.Name).ToList() ?? new();

            HotelServices = data.HotelServices?.Select(s => new OwnerHotelServiceVM
            {
                Id = s.Id,
                ServiceId = s.ServiceId,
                ServiceName = s.ServiceName,
                Description = s.Description,
                Price = s.Price,
                Unit = s.Unit,
                IsActive = s.IsActive
            }).ToList() ?? new();

            // === [FIX QUAN TRỌNG] MAP ROOM TYPES ===
            RoomTypes = data.RoomTypes?.Select(rt => new RoomTypeDetailVM
            {
                Id = rt.Id,
                Name = rt.Name,
                PricePerNight = rt.PricePerNight,
                Quantity = rt.Quantity,
                Area = rt.Area,
                AdultCapacity = rt.AdultCapacity,
                ChildCapacity = rt.ChildCapacity,
                Description = rt.Description,
                DefaultImageUrl = rt.DefaultImageUrl,

                // Map Amenities của Room (nếu có)
                Amenities = rt.Amenities?.Select(a => new AmenityVM
                {
                    Id = a.Id,
                    Name = a.Name,
                    Additional = a.Additional
                }).ToList() ?? new(),
                SelectedAmenityIds = new HashSet<int>(rt.Amenities?.Select(a => a.Id) ?? new List<int>()),

                // Map Beds
                Beds = rt.Beds?.Select(b => new RoomBedTypesVM
                {
                    BedTypeId = b.BedTypeId,
                    Quantity = b.Quantity
                }).ToList() ?? new(),

                // Map Views
                Views = rt.Views?.Select(v => new RoomViewTypesVM
                {
                    ViewTypeId = v.ViewTypeId,
                    ViewTypeName = v.ViewTypeName
                }).ToList() ?? new(),
                SelectedViewIds = new HashSet<int>(rt.Views?.Select(v => v.ViewTypeId) ?? new List<int>())
            }).ToList() ?? new();

            // Lưu dữ liệu mới vào LocalStorage
            await PersistAsync();
            Notify();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("LoadDraft Error: " + ex.Message);
            return false;
        }
    }

    // XÓA DRAFT (khi submit hoặc hủy)
    public async Task ClearDraftAsync()
    {
        DraftHotelId = 0;
        BasicInfo = new();
        Images = new();
        SelectedAmenityIds.Clear();
        SelectedPolicyByType.Clear();
        CustomPolicies.Clear();
        HotelServices.Clear();
        await _localStorage.RemoveItemAsync(STORAGE_KEY);
        Notify();
    }


    // BƯỚC 1: TẠO DRAFT
    public async Task<bool> CreateDraftAsync()
    {
        if (IsDraftCreated) return true;

        var dto = new HotelCreateOrUpdateVM
        {
            Name = BasicInfo.Name.Trim(),
            Address = BasicInfo.Address.Trim(),
            CityId = BasicInfo.CityId,
            Description = BasicInfo.Description?.Trim(),
            AccommodationTypeId = BasicInfo.AccommodationTypeId,
            ChainId = BasicInfo.ChainId,
            ContactName = BasicInfo.ContactName?.Trim(),
            ContactPhone = BasicInfo.ContactPhone?.Trim(),
            ContactEmail = BasicInfo.ContactEmail?.Trim(),
            // các field khác nếu cần
        };

        await SetAuthHeader();
        var response = await _httpClient.PostAsJsonAsync("hotel/owner/create-hotel", dto);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            _snackbar.Add("Không thể tạo khách sạn", Severity.Error);
            return false;
        }

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<HotelCreateOrUpdateVM>>();
        if (result?.StatusCode == StatusCodeResponse.Success && result.Content?.HotelId > 0)
        {
            DraftHotelId = result.Content.HotelId;
            BasicInfo.HotelId = DraftHotelId;
            HotelServices = new();
            RoomTypes = new();
            SelectedAmenityIds.Clear();
            await PersistAsync();
            Notify();
            _snackbar.Add($"Tạo draft thành công! ID: {DraftHotelId}", Severity.Success);
            return true;
        }

        _snackbar.Add(result?.Message ?? "Tạo draft thất bại", Severity.Error);
        return false;
    }

    public async Task UpdateBasicInfoAsync()
    {
        if (!IsDraftCreated) return;

        // Gọi API Update (giống API Create nhưng là PUT)
        await SetAuthHeader();
        // Giả sử bạn dùng chung DTO HotelCreateOrUpdateVM
        var response = await _httpClient.PutAsJsonAsync($"hotel/owner/update-hotel/{DraftHotelId}", BasicInfo);

        // Không cần thông báo thành công mỗi lần auto-save để tránh phiền
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine("Auto-save basic info failed");
        }
    }

    // cập nhập k cần load lại api
    public void UpdateLocalServiceList(OwnerHotelServiceVM service, bool isDelete = false)
    {
        if (HotelServices == null) HotelServices = new List<OwnerHotelServiceVM>();
        var index = HotelServices.FindIndex(x => x.ServiceId == service.ServiceId);
        if (isDelete)
        {
            if (index != -1) HotelServices.RemoveAt(index);
        }
        else
        {
            if (index != -1)
            {
                // UPDATE: Ghi đè thông tin mới
                // Giữ lại Id thực từ DB nếu service truyền vào đang bị 0 (đề phòng)
                if (service.Id == 0 && HotelServices[index].Id > 0)
                {
                    service.Id = HotelServices[index].Id;
                }
                HotelServices[index] = service;
            }
            else
            {
                // ADD
                HotelServices.Add(service);
            }
        }
        _ = PersistAsync();
        Notify();
    }

    public async Task RefreshRoomTypesAsync()
    {
        await LoadRoomTypesAsync();
        Notify();
    }

    public async Task<int?> CloneRoomTypeAsync(int sourceRoomTypeId)
    {
        if (DraftHotelId == 0) return null;

        try
        {
            await SetAuthHeader();
            // Gọi API Clone ở Backend
            var response = await _httpClient.PostAsync($"hotel/owner/roomtype/clone/{sourceRoomTypeId}", null);

            if (response.IsSuccessStatusCode)
            {
                // Ta deserialize để lấy ID của phòng mới
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<RoomTypeDetailVM>>();

                if (result?.StatusCode == StatusCodeResponse.Success && result.Content != null)
                {
                    // Refresh lại danh sách phòng trong RAM
                    await LoadRoomTypesAsync();
                    Notify();

                    // Trả về ID mới để UI mở popup edit
                    return result.Content.Id;
                }
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _snackbar.Add($"Lỗi nhân bản: {error}", Severity.Error);
            }
        }
        catch (Exception ex)
        {
            _snackbar.Add($"Lỗi: {ex.Message}", Severity.Error);
        }

        return null;
    }

    public async Task<bool> DeleteRoomTypeAsync(int roomTypeId)
    {
        if (DraftHotelId == 0) return false;

        await SetAuthHeader();
        var response = await _httpClient.DeleteAsync($"hotel/owner/roomtype/{roomTypeId}"); // Đảm bảo API route đúng

        if (response.IsSuccessStatusCode)
        {
            // Xóa thành công thì load lại danh sách luôn
            await LoadRoomTypesAsync();
            Notify();
            return true;
        }
        return false;
    }

    public async Task<bool> DeleteServiceAsync(int serviceId)
    {
        if (DraftHotelId == 0) return false;

        await SetAuthHeader();
        var response = await _httpClient.DeleteAsync($"hotel/owner/remove-service/{serviceId}");

        if (response.IsSuccessStatusCode)
        {
            // Xóa khỏi list trong RAM để cập nhật UI ngay
            var item = HotelServices.FirstOrDefault(x => x.Id == serviceId);
            if (item != null)
            {
                UpdateLocalServiceList(item, isDelete: true);
            }
            return true;
        }
        return false;
    }

    // UPLOAD ẢNH
    public async Task<bool> UploadCoverAsync(IBrowserFile file)
        => await UploadImageAsync(file, true);

    public async Task<bool> UploadGalleryAsync(IBrowserFile file)
        => await UploadImageAsync(file, false);

    private async Task<bool> UploadImageAsync(IBrowserFile file, bool isCover)
    {
        if (!IsDraftCreated)
        {
            _snackbar.Add("Chưa tạo khách sạn!", Severity.Warning);
            return false;
        }

        await SetAuthHeader();
        using var content = new MultipartFormDataContent();
        using var stream = file.OpenReadStream(10 * 1024 * 1024);
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

        content.Add(fileContent, "Image", file.Name);
        var endpoint = isCover
            ? $"hotel/owner/{DraftHotelId}/cover-image"
            : $"hotel/owner/{DraftHotelId}/gallery-image";

        var response = await _httpClient.PostAsync(endpoint, content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            _snackbar.Add($"Upload thất bại: {response.StatusCode}", Severity.Error);
            return false;
        }

        var res = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
        if (res?.StatusCode != StatusCodeResponse.Success || string.IsNullOrEmpty(res.Content))
        {
            _snackbar.Add("Upload thất bại: URL không hợp lệ", Severity.Error);
            return false;
        }

        if (isCover)
            Images.CoverImageUrl = res.Content;
        else
            Images.GalleryImageUrls.Add(res.Content);

        await PersistAsync();
        Notify();
        return true;
    }

    // XÓA ẢNH
    public async Task<bool> DeleteImageAsync(string url)
    {
        if (!IsDraftCreated) return false;

        await SetAuthHeader();
        var response = await _httpClient.DeleteAsync($"hotel/owner/{DraftHotelId}/del-image?imageUrl={Uri.EscapeDataString(url)}");
        if (response.IsSuccessStatusCode)
        {
            if (Images.CoverImageUrl == url)
            {
                Images.CoverImageUrl = null;
            }
            // Không còn string trong GalleryImageUrls → không cần Remove(url)

            await PersistAsync();
            Notify();
            return true;
        }

        _snackbar.Add("Xóa ảnh thất bại", Severity.Error);
        return false;
    }

    public async Task SyncImagesFromServerAsync()
    {
        if (!IsDraftCreated) return;

        try
        {
            await SetAuthHeader();
            var resp = await _httpClient.GetFromJsonAsync<ApiResponse<HotelImagesVM>>(
                $"hotel/owner/{DraftHotelId}/images");

            if (resp?.StatusCode == StatusCodeResponse.Success && resp.Content != null)
            {
                // ĐÈ HOÀN TOÀN DỮ LIỆU THẬT TỪ SERVER
                Images.CoverImageUrl = resp.Content.CoverImageUrl;
                Images.GalleryImageUrls = new List<string>(resp.Content.GalleryImageUrls);

                // Ghi đè localStorage luôn để lần sau mở nhanh hơn
                await PersistAsync();
                Notify();
            }
        }
        catch (Exception ex)
        {
            // Không crash nếu mạng lỗi, vẫn dùng dữ liệu cũ
            Console.WriteLine("Sync images failed: " + ex.Message);
        }
    }

    // GỬI DUYỆT
    public async Task<bool> SubmitForReviewAsync()
    {
        if (!IsDraftCreated) return false;

        await SetAuthHeader();
        var response = await _httpClient.PostAsync($"hotel/owner/{DraftHotelId}/submit", null);

        if (response.IsSuccessStatusCode)
        {
            var res = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
            await _localStorage.RemoveItemAsync(STORAGE_KEY); // xóa draft
            return true;
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            var apiRes = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
            _snackbar.Add(apiRes?.Message ?? "Gửi duyệt thất bại", Severity.Error);
            return false;
        }
    }

    // Gọi sau mỗi bước quan trọng (amenities, policies)
    private async Task SaveStepAsync(string endpoint, object data)
    {
        if (!IsDraftCreated) return;

        await SetAuthHeader();
        var response = await _httpClient.PostAsJsonAsync($"hotel/owner/{DraftHotelId}/{endpoint}", data);
        if (!response.IsSuccessStatusCode)
        {
            _snackbar.Add("Lưu bước này thất bại!", Severity.Error);
        }
    }

    // Lưu tiện ích
    public async Task SaveAmenitiesAsync()
    {
        if (!IsDraftCreated || !SelectedAmenityIds.Any()) return;
        var dto = new { AmenityIds = SelectedAmenityIds.ToList() };
        await SaveStepAsync("select-amenities", dto);
    }

    // Lưu chính sách
    public async Task SavePoliciesAsync()
    {
        if (!IsDraftCreated) return;
        var dto = new
        {
            PolicyIds = SelectedPolicyByType
                .Where(x => x.Value.HasValue)
                .Select(x => x.Value!.Value) // Chỉ lấy ID chính sách
                .ToList(),

            // SelectedPolicies = SelectedPolicyByType
            // .Where(x => x.Value.HasValue)
            // .Select(x => new { PolicyTypeId = x.Key, PolicyId = x.Value!.Value })
            // .ToList(),
            OwnerCustomPolicies = CustomPolicies
            .Select(p => new { Name = p, PolicyTypeId = 0 })
            .ToList()
        };
        await SaveStepAsync("select-policies", dto);
    }

    private async Task SetAuthHeader()
    {
        try
        {
            var token = await _localStorage.GetItemAsync<string>("accessToken");
            if (!string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        catch (JSDisconnectedException) { /* Bỏ qua nếu mất kết nối */ }
    }

}