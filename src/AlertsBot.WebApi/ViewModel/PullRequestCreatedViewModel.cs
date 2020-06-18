using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AlertsBot.WebApi.Model;


namespace AlertsBot.WebApi.ViewModel
{
    public class PullRequestCreatedViewModel
    {
        [DisplayName("id")]
        public string id { get; set; }
        [DisplayName("eventType")]
        public string eventType { get; set; }
        [DisplayName("publisherId")]
        public string publisherId { get; set; }
        [DisplayName("scope")]
        public string scope { get; set; }
        [DisplayName("message")]
        public Message message { get; set; }
        [DisplayName("detailedMessage")]
        public DetailedMessage detailedMessage { get; set; }
        [DisplayName("resource")]
        public Resource resource { get; set; }
        [DisplayName("resourceVersion")]
        public string resourceVersion { get; set; }
        [DisplayName("resourceContainers")]
        public ResourceContainers resourceContainers { get; set; }
        [DisplayName("createdDate")]
        public DateTime createdDate { get; set; }
    }
}
