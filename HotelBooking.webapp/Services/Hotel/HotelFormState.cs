// Services/HotelFormState.cs
using System.Net.Http.Headers;
using Blazored.LocalStorage;
using HotelBooking.Client.Models;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.Text.Json;
public sealed class HotelFormState
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly ISnackbar _snackbar;
    private const string STORAGE_KEY = HotelDraftSavedState.STORAGE_KEY;

    public HotelFormState(IHttpClientFactory factory, ILocalStorageService localStorage, ISnackbar snackbar)
    {
        _httpClient = factory.CreateClient("HotelBookingAPI");
        _localStorage = localStorage;
        _snackbar = snackbar;
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
    public List<CityVM> Cities {get;private set;} = new();
    public List<AmenityVM> AllAmenities { get; set; } = new();
    public List<PolicyTypeVM> PolicyGroups { get; set; } = new();

    public event Action? OnChange;

    public void Notify()
    {
        OnChange?.Invoke();
    }

    // load city
    public async Task LoadCitiesAsync()
    {
        try
        {
            await SetAuthHeader();
            var response = await _httpClient.GetAsync("hotel/get-cityName");
            var cities = await response.Content.ReadFromJsonAsync<List<CityVM>>();

            if (cities != null && cities.Any())
            {
                Cities = cities;
                BasicInfo.Cities = Cities;
                Notify(); // cập nhật MudSelect ngay lập tức
            }
            else
            {
                _snackbar.Add("Danh sách thành phố trống", Severity.Info);
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
            Notify();

            if (IsDraftCreated)
            {
                await SyncImagesFromServerAsync();
            }
        }
    }

    // Lưu vào localStorage mỗi khi thay đổi quan trọng
    private async Task PersistAsync()
    {
        var toSave = new HotelDraftSavedState
        {
            DraftHotelId = DraftHotelId,
            BasicInfo = BasicInfo,
            Images = Images,
            SelectedAmenityIds = SelectedAmenityIds.ToList(),
            SelectedPolicyByType = SelectedPolicyByType.Where(x => x.Value.HasValue).ToDictionary(x => x.Key, x => x.Value!.Value),
            CustomPolicies = CustomPolicies.ToList(),
            Timestamp = DateTime.UtcNow
        };
        await _localStorage.SetItemAsync(STORAGE_KEY, toSave);
    }

    public async Task<bool> LoadDraftAsync(int hotelId)
    {
        await SetAuthHeader();
        var resp = await _httpClient.GetFromJsonAsync<ApiResponse<HotelDraftVM>>(
            $"hotel/owner/{hotelId}/draft");

        if (resp?.StatusCode != StatusCodeResponse.Success || resp.Content == null)
        {
            _snackbar.Add("Không tải được draft.", Severity.Error);
            return false;
        }

        var d = resp.Content;

        // BASIC
        BasicInfo = d.BasicInfo;
        BasicInfo.HotelId = hotelId;

        // IMAGES
        Images = d.Images;

        // AMENITIES
        SelectedAmenityIds = new HashSet<int>(d.SelectedAmenityIds);

        // POLICIES
        SelectedPolicyByType = d.SelectedPolicyIds
            .ToDictionary(id => id, id => (int?)id);

        CustomPolicies = d.CustomPolicies.Select(c => c.Name).ToList();

        DraftHotelId = hotelId;
        await PersistAsync();
        Notify();
        return true;
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
            await PersistAsync();
            Notify();
            _snackbar.Add($"Tạo draft thành công! ID: {DraftHotelId}", Severity.Success);
            return true;
        }

        _snackbar.Add(result?.Message ?? "Tạo draft thất bại", Severity.Error);
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

    // Gọi sau khi tạo draft thành công
    public void SetDraftHotelId(int hotelId)
    {
        DraftHotelId = hotelId;
        Notify();
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
            SelectedPolicies = SelectedPolicyByType
            .Where(x => x.Value.HasValue)
            .Select(x => new { PolicyTypeId = x.Key, PolicyId = x.Value!.Value })
            .ToList(),
            OwnerCustomPolicies = CustomPolicies
            .Select(p => new { Name = p, PolicyTypeId = 0 })
            .ToList()
        };
        await SaveStepAsync("select-policies", dto);
    }

    private async Task SetAuthHeader()
    {
        var token = await _localStorage.GetItemAsync<string>("accessToken");
        if (!string.IsNullOrEmpty(token))
            _httpClient.DefaultRequestHeaders.Authorization = new("Bearer", token);
    }

    // 
    public bool BasicInfoIsValid =>
    !string.IsNullOrWhiteSpace(BasicInfo.Name) &&
    !string.IsNullOrWhiteSpace(BasicInfo.Address) &&
    BasicInfo.CityId > 0 &&
    !string.IsNullOrWhiteSpace(BasicInfo.ContactPhone);
    public bool ImagesIsValid => Images.IsComplete;
}