using Blazored.LocalStorage;
using MudBlazor;
using System.Text.Json;

public sealed class BookingState
{
    private readonly ILocalStorageService _localStorage;
    private readonly ISnackbar _snackbar;
    private const string STORAGE_KEY = "user_booking_cart";
    private const int EXPIRY_MINUTES = 15;
    // Event để UI cập nhật
    public event Action? OnChange;

    // --- DỮ LIỆU (Properties) ---
    // Ta set private set để bắt buộc phải update qua hàm (để còn trigger Save)
    public int HotelId { get; private set; }
    public string HotelName { get; private set; } = "";
    public string HotelImage { get; private set; } = "";
    public DateTime CheckIn { get; private set; } = DateTime.Today;
    public DateTime CheckOut { get; private set; } = DateTime.Today.AddDays(1);

    public int Adults { get; private set; } = 1;
    public int Children { get; private set; } = 0;
    public DateTime? SessionStartTime { get; private set; }
    public List<CartItemVM> SelectedRooms { get; private set; } = new();
    public BookingState(ILocalStorageService localStorage, ISnackbar snackbar)
    {
        _localStorage = localStorage;
        _snackbar = snackbar;
    }

    // --- 1. HÀM KHỞI TẠO (Gọi ở MainLayout hoặc trang RoomList) ---
    public async Task InitAsync()
    {
        try
        {
            // Tải từ LocalStorage lên
            var savedState = await _localStorage.GetItemAsync<BookingCartVM>(STORAGE_KEY);

            if (savedState != null)
            {
                // Kiểm tra dữ liệu có quá cũ không (VD: quá 15p thì xóa)
                if (DateTime.UtcNow - savedState.Timestamp > TimeSpan.FromMinutes(15))
                {
                    await ClearAsync();
                    _snackbar.Add("Hết thời gian giữ phòng, vui lòng chọn phòng lại.", Severity.Info);
                    return;
                }

                // Restore lại state
                HotelId = savedState.HotelId;
                HotelName = savedState.HotelName;
                HotelImage = savedState.HotelImage ?? "";
                CheckIn = savedState.CheckIn;
                CheckOut = savedState.CheckOut;
                Adults = savedState.Adults;
                Children = savedState.Children;
                SelectedRooms = savedState.SelectedRooms ?? new();
                SessionStartTime = savedState.SessionStartTime;
                NotifyStateChanged();
            }
        }
        catch (Exception ex)
        {
            _snackbar.Add($"Lỗi load giỏ hàng: {ex.Message}");
            await ClearAsync(); // Xóa dữ liệu hỏng
        }
    }

    public async Task StartSessionAsync()
    {
        if (SessionStartTime == null)
        {
            SessionStartTime = DateTime.Now;
            await PersistAsync(); // Lưu ngay vào Storage
        }
    }

    public async Task ResetSessionAsync()
    {
        SessionStartTime = null;
        await PersistAsync();
    }
    // --- 2. CÁC HÀM THAO TÁC DỮ LIỆU ---

    // Cập nhật thông tin tìm kiếm (Ngày, Số người)
    public async Task UpdateSearchCriteriaAsync(int hotelId, string hotelName, string hotelImage, DateTime inDate, DateTime outDate, int adults, int children)
    {
        // Nếu user đổi sang khách sạn khác -> Xóa giỏ hàng cũ
        if (HotelId != 0 && HotelId != hotelId)
        {
            if (SelectedRooms.Any())
            {
                _snackbar.Add("Đơn đặt phòng đã được làm mới.", Severity.Info);
            }
            SelectedRooms.Clear();
        }

        HotelId = hotelId;
        HotelName = hotelName;
        HotelImage = hotelImage ?? "";
        CheckIn = inDate;
        CheckOut = outDate;
        Adults = adults;
        Children = children;
        SessionStartTime = null;
        await PersistAsync(); // Lưu ngay
        NotifyStateChanged();
    }

    // Thêm/Sửa số lượng phòng
    public async Task AddOrUpdateRoomAsync(CartItemVM roomItem)
    {

        if (roomItem.Quantity <= 0)
        {
            var existing = SelectedRooms.FirstOrDefault(r => r.RoomTypeId == roomItem.RoomTypeId);
            if (existing != null) SelectedRooms.Remove(existing);
        }
        else
        {
            var existing = SelectedRooms.FirstOrDefault(r => r.RoomTypeId == roomItem.RoomTypeId);
            if (existing != null)
            {
                // Update
                existing.Quantity = roomItem.Quantity;
                existing.PricePerNight = roomItem.PricePerNight; // Cập nhật giá mới nhất nếu có
                existing.RoomName = roomItem.RoomName;
            }
            else
            {
                // Add new
                SelectedRooms.Add(roomItem);
            }
        }
        SessionStartTime = null;
        await PersistAsync(); // Lưu ngay
        NotifyStateChanged();
    }

    // Xóa sạch (khi đặt thành công)
    public async Task ClearAsync()
    {
        HotelId = 0;
        HotelName = "";
        HotelImage = "";
        SelectedRooms.Clear();

        await _localStorage.RemoveItemAsync(STORAGE_KEY);
        NotifyStateChanged();
    }

    // --- 3. LOGIC LƯU TRỮ (PERSIST) ---
    private async Task PersistAsync()
    {
        var stateToSave = new BookingCartVM
        {
            HotelId = HotelId,
            HotelName = HotelName,
            HotelImage = HotelImage,
            CheckIn = CheckIn,
            CheckOut = CheckOut,
            Adults = Adults,
            Children = Children,
            SelectedRooms = SelectedRooms,
            SessionStartTime = SessionStartTime,
            Timestamp = DateTime.UtcNow
        };

        await _localStorage.SetItemAsync(STORAGE_KEY, stateToSave);
    }

    private void NotifyStateChanged() => OnChange?.Invoke();

    // Tính tổng tiền (Property helper)
    public bool HasItems => SelectedRooms.Any();
    public int TotalRooms => SelectedRooms.Sum(r => r.Quantity);
    public decimal TotalPrice
    {
        get
        {
            int nights = Math.Max(1, (CheckOut - CheckIn).Days);
            return SelectedRooms.Sum(r => r.PricePerNight * r.Quantity * nights);
        }
    }

    public async Task RemoveRoomAsync(int roomTypeId)
    {
        var existing = SelectedRooms.FirstOrDefault(r => r.RoomTypeId == roomTypeId);
        if (existing != null)
        {
            SelectedRooms.Remove(existing);
            
            // [MỚI] Xóa bớt phòng cũng tính là đổi đơn hàng -> Reset
            SessionStartTime = null; 

            await PersistAsync();
            NotifyStateChanged();
        }
    }
}
