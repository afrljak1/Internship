using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Movie_Api.Models
{
    public class Login
    {

        [Key]
        public int UserId { get; set; }

        [Column(TypeName = "nvarchar(250)")]
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
