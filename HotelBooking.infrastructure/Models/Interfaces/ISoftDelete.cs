namespace HotelBooking.infrastructure.Models
{
    // Interface này định nghĩa hành vi "Xóa mềm"
    public interface ISoftDelete
    {
        // Property để kiểm tra trạng thái (Read-only từ bên ngoài càng tốt)
        bool? IsDeleted { get; }

        // HÀNH VI: Đây là Rich Model - Ra lệnh xóa
        void Delete();

        // HÀNH VI: Khôi phục
        // void UndoDelete();
    }
}