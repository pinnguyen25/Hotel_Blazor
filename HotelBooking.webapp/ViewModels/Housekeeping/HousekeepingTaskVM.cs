// Hiển thị danh sách task
public class HousekeepingTaskVM
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string RoomStatus { get; set; } = string.Empty; // Cleaning, Maintenance...
    public int? StaffId { get; set; }
    public string StaffName { get; set; } = "Chưa giao";
    public string TaskStatus { get; set; } = string.Empty; // Pending, Assigned, Cleaning, Completed
    public string Priority { get; set; } = "Normal";
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

// Gửi yêu cầu giao việc
public class AssignTaskRequestVM
{
    public int TaskId { get; set; }
    public int StaffId { get; set; }
    public string Priority { get; set; } = "Normal"; // Low, Normal, High
    public string? Note { get; set; }
}

// Gửi yêu cầu cập nhật trạng thái (Staff làm)
public class UpdateTaskStatusRequestVM
{
    public int TaskId { get; set; }
    public string Status { get; set; } = string.Empty; // Cleaning, Completed, Maintenance
    public string? IssueDescription { get; set; } // Nếu báo hỏng
}