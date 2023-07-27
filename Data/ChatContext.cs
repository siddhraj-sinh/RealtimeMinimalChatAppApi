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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Message>().ToTable("Messages");

            //configure receiver
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.NoAction);
            //configure sender
            modelBuilder.Entity<Message>()
              .HasOne(m => m.Sender)
              .WithMany()
              .HasForeignKey(m => m.SenderId)
              .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
