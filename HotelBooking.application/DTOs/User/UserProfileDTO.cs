public class UserProfileDTO
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; } // Location
    public string AvatarUrl { get; set; }
    public DateOnly? DateOfBirth { get; set; }
}