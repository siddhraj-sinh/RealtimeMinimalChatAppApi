using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MinimalChatAppApi.Models;

namespace MinimalChatAppApi.Data
{
    public class ChatContext:IdentityDbContext<IdentityUser>
    {

        public ChatContext(DbContextOptions<ChatContext> options) : base(options)
        {
        }
   

        public DbSet<LogModel> Log { get; set; }
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    //configure table names
        //    modelBuilder.Entity<LogModel>().ToTable("Logss");
        //}
    }
}
