namespace ParsGreen.HttpService.Core.Api
{
    internal class ParsGreenUrlConfig
    {
        private const string RequestUrl = "https://login.parsgreen.com/UrlService/sendSMS.ashx?from={3}&to={0}&text={1}&signature={2}";

        public static string GetSendUrl(string token, string number, string body, string defaultNumber) =>
            string.Format(RequestUrl, number, body, token, defaultNumber);
    }
}