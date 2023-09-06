using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Movie_Api.Models
{
    public class Movie
    {
        [Key]
        public int MovieId { get; set; }

        [Column(TypeName = "nvarchar(250)")]
        public string Title { get; set; }
        public string Type { get; set; }
        public string? Year { get; set; }
        public string? Genre { get; set; }
        public string? ImdbId { get; set; }
        public string? Released { get; set; }
        public string? Language { get; set; }
        public string? Country { get; set; }
        public string? Awards { get; set; }
        public string? Plot { get; set; }
        public string? Writer { get; set; }
    }

}



