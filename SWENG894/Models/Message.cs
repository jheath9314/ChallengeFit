using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.Models
{
    public class Message
    {
        public int Id { get; set; }

        public string SentById { get; set; }
        public ApplicationUser SentBy { get; set; }

        public string SentToId { get; set; }
        public ApplicationUser SentTo { get; set; }

        [Required(ErrorMessage = "Subject is required.")]
        [MaxLength(50, ErrorMessage = "Subject cannot exceed 50 characters.")]
        public string Subject { get; set; }

        [NotMapped]
        public string SubjectPreview => Subject.Length > 7 ? Subject.Substring(0, 7) + "..." : Subject;

        [Required(ErrorMessage = "Message is required.")]
        [Display(Name = "Message")]
        [MaxLength(500, ErrorMessage = "Message cannot exceed 500 characters.")]
        public string Body { get; set; }

        [NotMapped]
        public string BodyPreview => Body.Length > 7 ? Body.Substring(0, 7) + "..." : Body;

        [Required]
        [Display(Name = "Sent Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy h:mm}")]
        [DataType(DataType.DateTime)]
        public DateTime SentTime { get; set; }

        public MessageSendStatud SendStatus { get; set; }

        [NotMapped]
        public bool MessageSent => SendStatus == MessageSendStatud.Sent;

        public MessageReadStatud ReadStatus { get; set; }

        [NotMapped]
        public bool MessageRead => ReadStatus == MessageReadStatud.Read;

        public MessageTypes MessageType { get; set; }

        public bool DeletedBySender { get; set; }

        public bool DeletedByReceiver { get; set; }

        public enum MessageSendStatud
        {
            New,
            Sent
        }

        public enum MessageReadStatud
        {
            New,
            Read
        }

        public enum MessageTypes
        {
            Correspondence,
            FriendRequest
        }
    }
}
