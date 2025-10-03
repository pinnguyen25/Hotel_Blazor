public class CurrentUserVM
{
    public bool IsAuthenticated { get; set; }
    public string? FullName { get; set; }
    public string? Avatar { get; set; }
    public List<string>? Roles { get; set; }
    public int? UserId { get; set; }
}