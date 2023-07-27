using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalChatAppApi.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Sender")]
        [Required]
        public string SenderId { get; set; }

        [ForeignKey("Receiver")]
        [Required]
        public string ReceiverId { get; set; }

        [Required]
        public string MessageContent { get; set; }

        public DateTime Timestamp { get; set; }

        // Navigation properties
        public IdentityUser Sender { get; set; }
        public IdentityUser Receiver { get; set; }
    }

}
