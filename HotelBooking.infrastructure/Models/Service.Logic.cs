using System.Text.Json;
using System.ComponentModel.DataAnnotations; // Để dùng ValidationException nếu muốn
namespace HotelBooking.infrastructure.Models;

public partial class Service : ISoftDelete
{
    public void Delete()
    {
        if (this.IsDeleted == true) return;

        // Logic xóa nằm gọn ở đây
        this.IsDeleted = true;

        // Nếu sau này muốn thêm logic: Set ngày xóa, đổi status...
        // Chỉ cần sửa ở đây, BaseManage không bị ảnh hưởng.
        // this.DeletedAt = DateTime.UtcNow;
    }

    // 1. Cấu hình JSON nội bộ (Helper Config)
    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
    };

    #region FACTORY METHODS (Create - Thay thế việc new bên ngoài)
    // =================================================================
    // FACTORY METHODS (Create - Thay thế việc new bên ngoài)
    // =================================================================
    public static Service CreateStandard(string name, string? des, decimal price, string unit)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Tên dịch vụ không được để trống.");
        }

        if (string.IsNullOrEmpty(unit)) throw new ArgumentException("Đơn vị không được để trống");

        var service = new Service();
        service.ConfigureStandard(name, des, price, unit);
        service.IsDeleted = false;
        return service;
    }

    public static Service CreateAirportTransfer(string name, string? description, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Tên dịch vụ không được để trống.");
        }

        var service = new Service();
        service.ConfigureAirportTransfer(name, description, price, null); // Null data khi tạo mới
        service.IsDeleted = false;
        return service;
    }
    #endregion

    #region BEHAVIOR METHODS (Update - Đóng gói logic đa hình)
    // =================================================================
    // BEHAVIOR METHODS (Update - Đóng gói logic đa hình)
    // =================================================================

    /// <summary>
    /// Hành vi dành cho loại Standard: Tự đóng gói Unit vào JSON
    /// </summary>
    public void ConfigureStandard(string name, string? des, decimal price, string unit)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Tên dịch vụ không được để trống.");
        }

        if (string.IsNullOrEmpty(unit)) throw new ArgumentException("Đơn vị không được để trống");

        this.ServiceTypeId = (int)ServiceTypeEnum.Standard;
        this.Name = name;
        this.Description = des;
        this.Price = price;

        var data = new { Unit = unit };
        this.Additional = JsonSerializer.Serialize(data, _jsonOptions);
    }

    /// <summary>
    /// Hành vi dành cho loại Airport: Tự xử lý dữ liệu phức tạp
    /// </summary>
    public void ConfigureAirportTransfer(string name, string? description, decimal price, AirportTransferDataModel? data)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Tên dịch vụ không được để trống.");
        }

        this.ServiceTypeId = (int)ServiceTypeEnum.AirportTransfer;
        this.Name = name;
        this.Description = description;
        this.Price = price;

        // INTERNAL HELPER LOGIC:
        // Nếu có data mới truyền vào thì update, nếu không (null) thì giữ nguyên JSON cũ hoặc tạo mặc định
        if (data != null)
        {
            this.Additional = JsonSerializer.Serialize(data, _jsonOptions);
        }
        else if (string.IsNullOrEmpty(this.Additional) || this.Additional == "{}")
        {
            // Tạo mặc định nếu chưa có
            this.Additional = JsonSerializer.Serialize(new AirportTransferDataModel(), _jsonOptions);
        }
    }
    #endregion

    #region READ HELPERS (Hỗ trợ đọc dữ liệu ra DTO)
    // =================================================================
    // READ HELPERS (Hỗ trợ đọc dữ liệu ra DTO)
    // =================================================================

    // Helper lấy Unit (Standard)
    public string GetStandardUnit()
    {
        if (string.IsNullOrEmpty(this.Additional)) return "";

        try
        {
            var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(this.Additional, _jsonOptions);
            return dict != null && dict.TryGetValue("Unit", out var u) ? u : "";
        }
        catch { return ""; }
    }

    // Helper lấy Data (Airport)
    public AirportTransferDataModel GetAirportData()
    {
        if (string.IsNullOrEmpty(this.Additional)) return new AirportTransferDataModel();

        try
        { return JsonSerializer.Deserialize<AirportTransferDataModel>(this.Additional, _jsonOptions) ?? new AirportTransferDataModel(); }
        catch { return new AirportTransferDataModel(); }
    }

    #endregion
}

// Class POCO hỗ trợ lưu trữ JSON (Đặt trong Infra để Entity dùng)
public class AirportTransferDataModel
{
    public int? MaxPassengers { get; set; }
    public int? MaxLuggage { get; set; }
    public decimal? RoundTripPrice { get; set; }
    public decimal? AdditionalFee { get; set; }
    public TimeSpan? AdditionalFeeStartTime { get; set; }
    public TimeSpan? AdditionalFeeEndTime { get; set; }
}

public enum ServiceTypeEnum
{
    Standard = 1,
    AirportTransfer = 2,
}