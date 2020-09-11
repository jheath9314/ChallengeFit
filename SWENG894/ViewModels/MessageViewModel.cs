using SWENG894.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.ViewModels
{
    public class MessageViewModel
    {
        public string SentById { get; set; }
        public ApplicationUser SentBy { get; set; }

        [Display(Name = "Send To")]
        public string SentToId { get; set; }
        public ApplicationUser SentTo { get; set; }

        [Required(ErrorMessage = "Subject is required.")]
        [MaxLength(50, ErrorMessage = "Subject cannot exceed 50 characters.")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Message is required.")]
        [Display(Name = "Message")]
        [MaxLength(500, ErrorMessage = "Message cannot exceed 500 characters.")]
        public string Body { get; set; }

        [Display(Name = "Send To")]
        public string FullName { get; set; }
    }
}
