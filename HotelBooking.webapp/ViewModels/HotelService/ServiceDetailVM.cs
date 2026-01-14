public class ServiceDetailVM : ServicesVM // Kế thừa để có sẵn Id, Name...
    {
        public int UsageCount { get; set; }
        public decimal AveragePrice { get; set; }
        public List<HotelServiceUsageVM> UsedByHotels { get; set; } = new();
    }