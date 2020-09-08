using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.Models
{
    public class Message
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Subject is required.")]
        [MaxLength(50, ErrorMessage = "Subject cannot exceed 50 characters.")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Message is required.")]
        [Display(Name = "Message")]
        [MaxLength(500, ErrorMessage = "Message cannot exceed 500 characters.")]
        public string Body { get; set; }

        [Required]
        public string SenderId { get; set; }

        [Required]
        public string ReceiverId { get; set; }

        [Required(ErrorMessage = "Sent Date is required.")]
        [Display(Name = "Sent Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime SentDate { get; set; }

        enum MessageStatud
        {
            New,
            Sent
        }
    }
}
