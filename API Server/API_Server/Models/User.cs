using System.ComponentModel.DataAnnotations;

namespace API_Server.Models
{
    public class User
    {
        [Key] // Обозначает первичный ключ
        public int UserId { get; set; } // автоматически будет автоинкрементным

        public int? ColonyId { get; set; } // Опционально, если колония не обязательна

        [StringLength(20)] // Ограничение длины строки для username
        public string Username { get; set; }

        [StringLength(64)] // Ограничение длины строки для хешированного пароля
        public string HashedPassword { get; set; }

        [StringLength(40)] // Ограничение длины строки для email
        public string Email { get; set; }

        public int? PositionInTheLeaderboard { get; set; }
    }
}
