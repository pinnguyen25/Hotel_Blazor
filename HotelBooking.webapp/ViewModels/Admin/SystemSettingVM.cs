public class SystemSettingVM
{
    public string KeyWord { get; set; }
    public string SetTime { get; set; } // Map với cột DB
    public string Description { get; set; }
    public DateTime? UpdateAt { get; set; }
}