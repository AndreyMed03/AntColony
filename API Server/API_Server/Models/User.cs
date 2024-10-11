using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Server.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        public int? ColonyId { get; set; }
        public string? Username { get; set; }
        public string? HashedPassword { get; set; }
        public string? Email { get; set; }

        public int? PositionInTheLeaderboard { get; set; }
    }
}
