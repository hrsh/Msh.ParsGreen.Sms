using System;

namespace ParsGreen.HttpService.Core.Api.Models
{
    public class ResponseModel
    {
        public string Number { get; set; }

        public string Status { get; set; }

        public string ReferenceId { get; set; }

        public double Cost { get; set; }

        public override string ToString()
        {
            switch (Status)
            {
                case "0":
                    return "ارسال موفق";

                case "1":
                    return "ارسال ناموفق";

                case "2":
                    return "خطای سیستم";

                case "3":
                    return "بلک لیست";

                default:
                    throw new NotSupportedException(nameof(Status));
            }
        }
    }
}