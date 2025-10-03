using HotelBooking.webapp.ViewModels.Hotel;

public interface IHotelServices
{
    public Task<HotelListItemVM> GetHotelAsync();
    public Task<List<SearchSuggestionVM>> GetAutocompleteAsync(string keyword);
    public Task<List<SearchSuggestionVM>> GetCitiesAsync();
}

public class HotelServices : IHotelServices
{
    private readonly HttpClient _httpClient;

    public HotelServices(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("HotelBookingAPI");
    }

    public async Task<List<SearchSuggestionVM>> GetAutocompleteAsync(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword)) return new List<SearchSuggestionVM>();
        var res = await _httpClient.GetFromJsonAsync<List<SearchSuggestionVM>>($"Hotel/autocomplete?keyword={Uri.EscapeDataString(keyword)}");
        return res ?? new List<SearchSuggestionVM>();
    }

    public async Task<List<SearchSuggestionVM>> GetCitiesAsync()
    {
        var cities = await _httpClient.GetFromJsonAsync<List<CityVM>>("Hotel/get-cityName");

        if (cities == null) return new List<SearchSuggestionVM>();

        return cities.Select(c => new SearchSuggestionVM
        {
            Label = c.Name,
            Type = "City",
            RefId = c.Id
        }).ToList();
    }

    public Task<HotelListItemVM> GetHotelAsync()
    {
        throw new NotImplementedException();
    }
}