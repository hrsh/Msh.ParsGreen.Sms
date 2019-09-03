using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ParsGreen.HttpService.Core.Api.Models;

namespace ParsGreen.HttpService.Core.Api
{
    public class SmsProvider : ISmsProvider
    {
        private readonly string _token;
        private readonly int _maxLength;
        private readonly double _mci;
        private readonly double _irancell;
        private readonly double _rightel;
        private readonly string _defaultNumber;

        public SmsProvider(IOptionsSnapshot<ParsGreenConfiguration> options)
        {
            var parsGreenConfiguration = options;
            if (parsGreenConfiguration == null)
                throw new ArgumentNullException(nameof(parsGreenConfiguration));

            _token = parsGreenConfiguration.Value.Token;
            _maxLength = parsGreenConfiguration.Value.MaxLenght;
            _mci = parsGreenConfiguration.Value.MciCoefficent;
            _irancell = parsGreenConfiguration.Value.IrancellCoefficent;
            _rightel = parsGreenConfiguration.Value.RightelCoefficent;
            _defaultNumber = parsGreenConfiguration.Value.DefaultNumber;
            if (string.IsNullOrWhiteSpace(_token))
                throw new ArgumentNullException(nameof(_token));
        }

        public ParsGreenResult<ResponseModel> InvokeSendSms(SendSmsModel model)
        {
            var url = ParsGreenUrlConfig.GetSendUrl(_token, model.ToNumber, model.Body, _defaultNumber);
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "Get";
            HttpWebResponse httpResponse;
            try
            {
                httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            }
            catch
            {
                return ParsGreenResult<ResponseModel>.Failed(new ParsGreenError
                {
                    Code = "-1000",
                    Description = "Could not send request."
                });
            }
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream() ?? throw new InvalidOperationException()))
            {
                var result = streamReader.ReadToEnd();
                return PrepairModel(result);
            }
        }

        public async Task<ParsGreenResult<ResponseModel>> InvokeSendSmsAsync(SendSmsModel model)
        {
            var url = ParsGreenUrlConfig.GetSendUrl(_token, model.ToNumber, model.Body, _defaultNumber);
            if (model.ToNumber.Length != 11 || !model.ToNumber.StartsWith("09"))
                return ParsGreenResult<ResponseModel>.Failed(new ParsGreenError
                {
                    Code = "-1003",
                    Description = "Could not send message. Possibly not valid number."
                });

            if (string.IsNullOrWhiteSpace(model.Body))
                return ParsGreenResult<ResponseModel>.Failed(new ParsGreenError
                {
                    Code = "-1004",
                    Description = "Could not send empty message."
                });

            if (model.Body.Length > _maxLength)
                return ParsGreenResult<ResponseModel>.Failed(new ParsGreenError
                {
                    Code = "-1003",
                    Description = "Could not send message larger than 60 chars"
                });

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "Get";
            HttpWebResponse httpResponse;
            try
            {
                httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
            }
            catch
            {
                return ParsGreenResult<ResponseModel>.Failed(new ParsGreenError
                {
                    Code = "-1000",
                    Description = "Could not send request."
                });
            }
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream() ?? throw new InvalidOperationException()))
            {
                var result = streamReader.ReadToEnd();
                return PrepairModel(result);
            }
        }

        private ParsGreenResult<ResponseModel> PrepairModel(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
                return ParsGreenResult<ResponseModel>.Failed(new ParsGreenError
                {
                    Code = "-1001",
                    Description = "Could not get response from server."
                });

            if (response == "-1")
                return ParsGreenResult<ResponseModel>.Failed(new ParsGreenError
                {
                    Code = "-1002",
                    Description = "Token is not valid"
                });

            var segments = response.Split(';');

            if (!segments.Any() || segments.Length != 3)
                return ParsGreenResult<ResponseModel>.Failed(new ParsGreenError
                {
                    Code = "-1003",
                    Description = "Could not send message. Possibly not valid number."
                });

            var number = segments[0];
            var status = segments[1];
            var reference = segments[2];

            var model = new ResponseModel
            {
                Number = number,
                ReferenceId = reference,
                Status = status,
                Cost = CalculateCost(number)
            };

            return ParsGreenResult<ResponseModel>.Invoke(model);
        }

        private double CalculateCost(string number)
        {
            if (string.IsNullOrWhiteSpace(number)) return 0;
            var o1 = new[] { "0910", "0911", "0912", "0913", "0914", "0915", "0916", "0917", "0918", "0919" };
            var o2 = new[] { "0901", "0902", "0930", "0933", "0935", "0936", "0937", "0938", "0939", "0920", "0921", "0922" };
            if (o1.Any(number.StartsWith))
                return _mci;
            if (o2.Any(number.StartsWith))
                return _irancell;
            return _rightel;
        }
    }
}