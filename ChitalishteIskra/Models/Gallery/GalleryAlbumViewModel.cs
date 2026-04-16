namespace ChitalishteIskra.Models.Gallery
{
    public class GalleryAlbumViewModel
    {
        public string Slug { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string CoverImageUrl { get; set; } = string.Empty;
        public int PhotoCount { get; set; }
    }
}
