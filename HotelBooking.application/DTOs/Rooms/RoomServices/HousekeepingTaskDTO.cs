// DTO hiển thị danh sách công việc (Cho Lễ tân & Housekeeper)
    public class HousekeepingTaskDTO
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string? RoomNumber { get; set; } // Hiển thị: "101"
        public string? RoomStatus { get; set; } // Trạng thái hiện tại của phòng (Dirty/Cleaning...)
        
        public int? StaffId { get; set; }
        public string? StaffName { get; set; } // Hiển thị: "Cô Lao Công A"
        
        public string? TaskStatus { get; set; } // Pending, Assigned, Completed...
        public string? Priority { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

    // DTO để Lễ tân giao việc
    public class AssignTaskRequestDTO
    {
        public int TaskId { get; set; }
        public int StaffId { get; set; } // ID của nhân viên dọn phòng
        public string? Note { get; set; }
        public string Priority { get; set; } = "Normal";
    }

    // DTO để Nhân viên cập nhật trạng thái (Bắt đầu / Xong)
    public class UpdateTaskStatusRequestDTO
    {
        public int TaskId { get; set; }
        public string? Status { get; set; } // "Cleaning", "Completed"
        public string? IssueDescription { get; set; } // Thêm ghi chú lỗi (VD: "Máy lạnh kêu to", "Vòi nước rỉ")
    }