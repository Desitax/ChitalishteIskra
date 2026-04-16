namespace ChitalishteIskra.Models.Gallery
{
    public class GalleryDetailsViewModel
    {
        public string Slug { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CoverImageUrl { get; set; } = string.Empty;
        public List<string> ImageUrls { get; set; } = new();
    }
}
