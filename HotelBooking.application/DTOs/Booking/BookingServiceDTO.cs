public class BookingServiceDTO
    {
        public int Id { get; set; } // Primary Key của bảng BookingServices
        
        // Thông tin Dịch vụ
        public int ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public string? Description { get; set; } // Mô tả ngắn (nếu cần)
        public string? Unit { get; set; } // Đơn vị tính (Lần/Khách/Vé...)

        // Tài chính
        public decimal Price { get; set; } // Giá tại thời điểm đặt (có thể khác giá hiện tại)
        public int Quantity { get; set; }
        public decimal Total => Price * Quantity; // Tính tổng tiền item này

        // Trạng thái
        public bool IsPaid { get; set; } // Đã thanh toán chưa? (Quan trọng khi Checkout)
        
        public int? BookingRoomId { get; set; } 
        public string? RoomNumber { get; set; } // Hiển thị: "101"
        public string? GuestName { get; set; }  // Hiển thị: "Nguyễn Văn A"
    }