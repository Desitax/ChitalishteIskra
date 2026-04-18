namespace ChitalishteIskra.Models.Events
{
    public class EventsListViewModel
    {
        public List<EventIndexViewModel> Events { get; set; } = new();

        public string? SearchName { get; set; }

        public string? SearchDatePart { get; set; }

        public string? SearchDateType { get; set; }

        public string? SearchLocation { get; set; }

        public string? SearchStatus { get; set; }

        public List<string> NameSuggestions { get; set; } = new();

        public List<string> LocationSuggestions { get; set; } = new();
    }
}