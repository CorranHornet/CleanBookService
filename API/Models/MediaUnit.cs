using System.ComponentModel.DataAnnotations.Schema;

namespace BookService.Models
{

    [Table("MediaUnits")]
    public class MediaUnit
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public int? Number { get; set; }
        public int DurationMinutes { get; set; }
        public int MediaItemId { get; set; }
        public MediaItem? MediaItem { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}