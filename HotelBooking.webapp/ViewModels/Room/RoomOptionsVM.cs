using System.Text.Json.Serialization;

public class SelectOptionVM
{
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;
}

public class RoomOptionsVM
{
    [JsonPropertyName("bedTypes")]
    public List<SelectOptionVM> BedTypes { get; set; } = new();
    [JsonPropertyName("viewTypes")]
    public List<SelectOptionVM> ViewTypes { get; set; } = new();
}