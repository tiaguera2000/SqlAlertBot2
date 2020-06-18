using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AlertsBot.WebApi.ViewModel
{
    public class MessageViewModel
    {
        [Required(ErrorMessage = "The ChatId is Required")]
        [MinLength(1)]
        [MaxLength(100)]
        [DisplayName("ChatId")]
        public string ChatId { get; set; }

        [Required(ErrorMessage = "The Message is Required")]
        [MinLength(2)]
        [MaxLength(100)]
        [DisplayName("Message")]
        public string Message { get; set; }

    }
}
