using API_Server.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace API_Server.Contexts
{
    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }  // Таблица пользователей
        
    }
}
