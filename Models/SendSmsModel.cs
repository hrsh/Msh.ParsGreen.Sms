namespace ParsGreen.HttpService.Core.Api.Models
{
    public class SendSmsModel
    {
        public string ToNumber { get; set; }

        public string Body { get; set; }
    }
}